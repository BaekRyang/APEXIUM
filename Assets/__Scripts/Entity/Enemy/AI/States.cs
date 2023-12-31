using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public struct FloatPair
{
    public float x;
    public float y;

    public FloatPair(float _xValue, float _yValue)
    {
        x = _xValue;
        y = _yValue;
    }
}

[Serializable]
public abstract class State
{
    [SerializeField] [Inject] protected PlayerManager _playerManager;
    
    [SerializeField] protected EnemyAI enemyAI;
    
    protected static readonly int IsWalk   = Animator.StringToHash("IsWalk");
    protected static readonly int Attack   = Animator.StringToHash("Attack");
    protected static readonly int IsAttack = Animator.StringToHash("IsAttack");

    public State Initialize(EnemyAI _enemyAI)
    {
        DIContainer.Inject(this);
        enemyAI = _enemyAI;
        return this;
    }

    public bool IsReadyToAttack => enemyAI.waitForAttack == false;

    [SerializeField] protected string stateName = "NULL";

    public abstract void Enter();
    public abstract    void Execute();
    public abstract    void Exit();

    private const float CLIFF_DETECT_DISTANCE = 50f;

    protected static bool CliffDetect(EnemyAI _enemyAI) //절벽 감지 함수. 
    {
        int     _layerMask = 1 << LayerMask.NameToLayer("Floor");
        Vector3 _position  = _enemyAI.cachedTransform.position;

        Vector2 _checkPoint = new(
            _position.x + _enemyAI.targetDirection.x * _enemyAI.bodySize.x / 2,
            _position.y - _enemyAI.bodySize.y                              / 2
        );

        RaycastHit2D _hit = Physics2D.Raycast(_checkPoint,
                                              Vector2.down,
                                              CLIFF_DETECT_DISTANCE,
                                              _layerMask);
        Debug.DrawRay(_checkPoint, Vector2.down * CLIFF_DETECT_DISTANCE, Color.red);
        return _hit.collider == null;
    }

    protected static bool WallDetect(EnemyAI _enemyAI) //벽 감지 함수. 
    {
        int _layerMask = 1 << LayerMask.NameToLayer("Floor");

        RaycastHit2D _hit = Physics2D.Raycast(_enemyAI.cachedTransform.position,
                                              _enemyAI.targetDirection,
                                              _enemyAI.bodySize.x,
                                              _layerMask);

        Debug.DrawRay(_enemyAI.cachedTransform.position, _enemyAI.targetDirection * _enemyAI.bodySize.x, Color.blue);
        return _hit.collider != null;
    }

    /// <summary>
    /// Chase 범위 내에 플레이어가 있는지 확인하고 있다면 제일 가까운 플레이어 반환
    /// </summary>
    protected (Player, float) PlayerInRange(EnemyAI _enemyAI)
    {
        IEnumerable<Player> _players        = _playerManager.GetPlayersArray();
        Player              _targetPlayer   = null;
        float               _targetDistance = Mathf.Infinity;

        foreach (Player _player in _players)
        {
            float _distance = Vector2.Distance(_enemyAI.cachedTransform.position, _player.transform.position);
            if (!(_distance < _targetDistance)) continue;
            _targetDistance = _distance;
            _targetPlayer   = _player;
        }

        return _targetDistance <= _enemyAI.enemyBase.stats.chaseDistance ?
            (_targetPlayer, _targetDistance) :
            (null, -1);
    }

    protected static void Move(EnemyAI p_enemyAI)
    {
        p_enemyAI.cachedTransform.position +=
            p_enemyAI.targetDirection * (Time.deltaTime * p_enemyAI.enemyBase.stats.Speed);
    }
}

[Serializable]
public class SWander : State
{
    [Serializable]
    private enum WanderState
    {
        Moving,
        Waiting
    }

    [Space]
    [SerializeField] private WanderState currentState;

    [SerializeField] private float nextMovableTime, nextWaitTime;

    private readonly FloatPair _randomMovingTime = new(5, 10), //이동 시간 랜덤 범위
                               _randomWaitTime   = new(1, 5);  //대기 시간 랜덤 범위

    public override void Enter()
    {
        stateName               = "Wander";
        currentState            = WanderState.Moving;
        enemyAI.targetDirection = Vector3.right;
    }

    public override void Execute()
    {
        //플레이어가 Chase 범위 내에 있는지 확인해서 있으면 Chase 상태로 전환
        (Player _targetPlayer, float _targetDistance) = PlayerInRange(enemyAI);

        if (_targetDistance > 0)
        {
            enemyAI.targetPlayer = _targetPlayer;
            enemyAI.CurrentState = enemyAI.States["Chase"];
            return;
        }

        //----------------------------------------------

        switch (currentState)
        {
            case WanderState.Moving:
                if (Time.time >= nextWaitTime) //Wait로 전환할 시간이 되었다면
                {
                    //Waiting 상태로 진입
                    currentState = WanderState.Waiting;

                    //Waiting에서 Move로 돌아올 시간 지정
                    nextMovableTime = Time.time + Random.Range(_randomWaitTime.x, _randomWaitTime.y);
                    return;
                }

                if (CliffDetect(enemyAI))
                    enemyAI.targetDirection.InvertX(); //절벽 감지시 방향 반전

                if (WallDetect(enemyAI))
                    enemyAI.targetDirection.InvertX(); //벽 감지시 방향 반전

                enemyAI.targetDirection.Normalize();

                enemyAI.animator.SetBool(IsWalk, true);
                enemyAI.cachedTransform.position += enemyAI.targetDirection * (Time.deltaTime * enemyAI.enemyBase.stats.Speed);
                break;

            case WanderState.Waiting:
                if (Time.time >= nextMovableTime) //Move로 전환할 시간이 되었다면
                {
                    //Move 상태로 진입
                    currentState = WanderState.Moving;

                    //Move에서 Waiting으로 돌아올 시간 지정
                    nextWaitTime = Time.time + Random.Range(_randomMovingTime.x, _randomMovingTime.y);
                    return;
                }

                enemyAI.animator.SetBool(IsWalk, false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Exit()
    {
        Debug.Log("wander exit");
    }
}

public class SChase : State
{
    private const             float   CHASE_DIRECTION_CHANGE_DELAY = .5f;
    private                   UniTask _changeDirectionTask;


    public override void Enter()
    {
        Debug.Log("Chase Enter");
        stateName = "Chase";
    }

    public override void Execute()
    {
        //플레이어가 Chase 범위 내에 있는지 확인해서 없으면 Wander 상태로 전환
        (Player _targetPlayer, float _targetDistance) = PlayerInRange(enemyAI);

        if (_targetDistance < 0)
        {
            enemyAI.targetPlayer = null;
            enemyAI.CurrentState = enemyAI.States["Wander"];
            return;
        }

        //플레이어가 Attack 범위 내에 있는지 확인해서 있으면 Attack 상태로 전환
        if (IsReadyToAttack && //공격 준비가 안되었다면 그냥 따라붙도록 함
            _targetDistance <= enemyAI.enemyBase.stats.attackRange)
        {
            enemyAI.targetPlayer = _targetPlayer;
            enemyAI.CurrentState = enemyAI.States["Attack"];
            return;
        }

        //----------------------------------------------

        if (CliffDetect(enemyAI) || WallDetect(enemyAI))
            enemyAI.targetDirection *= -1; //절벽이나 벽 감지시 방향 반전

        //플레이어 쪽으로 방향 전환을 너무 자주하면 이상해 보이므로 딜레이를 줌
        if (_changeDirectionTask.Status != UniTaskStatus.Pending)
            _changeDirectionTask = ChangeDirectionToTarget();

        enemyAI.animator.SetBool(IsWalk, true);
        Move(enemyAI);
    }

    private async UniTask ChangeDirectionToTarget()
    {
        enemyAI.targetDirection = enemyAI.cachedTransform.position.DirectionToX(enemyAI.targetPlayer.PlayerPosition);
        await UniTask.Delay(TimeSpan.FromSeconds(CHASE_DIRECTION_CHANGE_DELAY));
    }

    public override void Exit()
    {
    }
}

public class SAttack : State
{
    [SerializeField] protected float lastAttackTime;
    [SerializeField] protected float nextAttackTime;

    protected               UniTask attackTask;


    public override void Enter()
    {
        stateName = "Attack";
        enemyAI.animator.SetBool(IsAttack, true);
    }

    public override void Execute()
    {
        if (attackTask.Status == UniTaskStatus.Pending) return;
        if (enemyAI.waitForAttack) AttackDelay();

        (Player _targetPlayer, float _targetDistance) = PlayerInRange(enemyAI);

        if (_targetDistance < 0)
        {
            enemyAI.targetPlayer = null;
            enemyAI.CurrentState = enemyAI.States["Wander"];
            return;
        }

        if (enemyAI.waitForAttack || //공격 대기중이거나 너무 멀어졌으면 Chase 상태로 전환
            _targetDistance > enemyAI.enemyBase.stats.attackRange)
        {
            enemyAI.CurrentState = enemyAI.States["Chase"];
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            lastAttackTime = Time.time;
            nextAttackTime = lastAttackTime + 1 / enemyAI.enemyBase.stats.AttackSpeed; //공격속도에 따라 다음 공격시간 계산

            attackTask = AttackAnimate();
        }
    }

    public override void Exit()
    {
        enemyAI.animator.SetBool("IsAttack", false);
    }

    protected async UniTask AttackAnimate()
    {
        enemyAI.animator.SetTrigger(Attack);

        enemyAI.animator.speed = enemyAI.enemyBase.stats.AttackSpeed * 2f;

        float _attackAnimationDelay = enemyAI.animator.GetCurrentAnimatorClipInfo(0).Length / enemyAI.animator.speed; //공격속도 영향을 받음
        await UniTask.Delay(TimeSpan.FromSeconds(_attackAnimationDelay));

        enemyAI.waitForAttack  = true;
        enemyAI.animator.speed = 1;
    }

    protected async void AttackDelay()
    {
        float _nextAttackDelay = nextAttackTime - Time.time;

        if (_nextAttackDelay > 0) //공격 타이밍이 이미 지나갔으면 대기할 필요가 없으므로 대기하지 않음 (음수 대기는 Exception 발생)
            await UniTask.Delay(TimeSpan.FromSeconds(_nextAttackDelay));
        else
            Debug.Log($"{_nextAttackDelay} : {nextAttackTime} - {Time.time}");

        enemyAI.waitForAttack = false;
    }
}
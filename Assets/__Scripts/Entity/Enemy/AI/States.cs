using System;
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

    public FloatPair(float xValue, float yValue2)
    {
        x = xValue;
        y = yValue2;
    }
}

[Serializable]
public abstract class State : IState
{
    [SerializeField] protected EnemyAI enemyAI;

    public State Initialize(EnemyAI p_enemyAI)
    {
        enemyAI = p_enemyAI;
        return this;
    }

    public bool IsReadyToAttack => enemyAI.waitForAttack == false;

    [SerializeField] protected string stateName = "NULL";

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();

    private const float CLIFF_DETECT_DISTANCE = 0.5f;

    protected static bool CliffDetect(EnemyAI p_enemyAI) //절벽 감지 함수. 
    {
        int     _layerMask = 1 << LayerMask.NameToLayer("Floor");
        Vector3 _position  = p_enemyAI._transform.position;

        Vector2 _checkPoint = new Vector2(
            _position.x + p_enemyAI._targetDirection.x * p_enemyAI.bodySize.x / 2,
            _position.y - p_enemyAI.bodySize.y                                / 2
        );

        RaycastHit2D _hit = Physics2D.Raycast(_checkPoint,
                                              Vector2.down,
                                              CLIFF_DETECT_DISTANCE,
                                              _layerMask);
        Debug.DrawRay(_checkPoint, Vector2.down * CLIFF_DETECT_DISTANCE, Color.red);
        Debug.Log($"Cliff Detect : {_hit.collider == null}");
        return _hit.collider == null;
    }

    protected static bool WallDetect(EnemyAI p_enemyAI) //벽 감지 함수. 
    {
        int _layerMask = 1 << LayerMask.NameToLayer("Floor");

        RaycastHit2D _hit = Physics2D.Raycast(p_enemyAI._transform.position,
                                              p_enemyAI._targetDirection,
                                              p_enemyAI.bodySize.x,
                                              _layerMask);

        Debug.DrawRay(p_enemyAI._transform.position, p_enemyAI._targetDirection * p_enemyAI.bodySize.x, Color.blue);
        Debug.Log($"Wall Detect : {_hit.collider != null}");
        return _hit.collider != null;
    }

    /// <summary>
    /// Chase 범위 내에 플레이어가 있는지 확인하고 있다면 제일 가까운 플레이어 반환
    /// </summary>
    protected static (Player, float) PlayerInRange(EnemyAI p_enemyAI)
    {
        Player[] _players        = GameManager.Instance.GetPlayers();
        Player   _targetPlayer   = null;
        float    _targetDistance = Mathf.Infinity;

        foreach (Player _player in _players)
        {
            float _distance = Vector2.Distance(p_enemyAI._transform.position, _player.transform.position);
            if (!(_distance < _targetDistance)) continue;
            _targetDistance = _distance;
            _targetPlayer   = _player;
        }

        return _targetDistance <= p_enemyAI._base.stats.chaseDistance ? (_targetPlayer, _targetDistance) : (null, -1);
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
    [SerializeField] private WanderState _currentState;

    [SerializeField] private float _nextMovableTime, _nextWaitTime;
    [SerializeField] private float _nextChangeDirectionTime;

    private readonly FloatPair _randomMovingTime = new FloatPair(5, 10), //이동 시간 랜덤 범위
                               _randomWaitTime   = new FloatPair(1, 5);  //대기 시간 랜덤 범위

    public override void Enter()
    {
        stateName                = "Wander";
        _currentState            = WanderState.Moving;
        enemyAI._targetDirection = Vector3.right;
    }

    public override void Execute()
    {
        //플레이어가 Chase 범위 내에 있는지 확인해서 있으면 Chase 상태로 전환
        (Player _targetPlayer, float _targetDistance) = PlayerInRange(enemyAI);

        if (_targetDistance > 0)
        {
            enemyAI._targetPlayer = _targetPlayer;
            enemyAI.CurrentState  = enemyAI.States["Chase"];
            return;
        }

        //----------------------------------------------

        switch (_currentState)
        {
            case WanderState.Moving:
                if (Time.time >= _nextWaitTime) //Wait로 전환할 시간이 되었다면
                {
                    //Waiting 상태로 진입
                    _currentState = WanderState.Waiting;

                    //Waiting에서 Move로 돌아올 시간 지정
                    _nextMovableTime = Time.time + Random.Range(_randomWaitTime.x, _randomWaitTime.y);
                    return;
                }

                if (CliffDetect(enemyAI))
                    enemyAI._targetDirection.InvertX(); //절벽 감지시 방향 반전

                if (WallDetect(enemyAI))
                    enemyAI._targetDirection.InvertX(); //벽 감지시 방향 반전

                enemyAI._targetDirection.Normalize();

                enemyAI._animator.SetBool("IsWalk", true);
                enemyAI._transform.position += enemyAI._targetDirection * (Time.deltaTime * enemyAI._base.stats.speed);
                break;

            case WanderState.Waiting:
                if (Time.time >= _nextMovableTime) //Move로 전환할 시간이 되었다면
                {
                    //Move 상태로 진입
                    _currentState = WanderState.Moving;

                    //Move에서 Waiting으로 돌아올 시간 지정
                    _nextWaitTime = Time.time + Random.Range(_randomMovingTime.x, _randomMovingTime.y);
                    return;
                }

                enemyAI._animator.SetBool("IsWalk", false);
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
            enemyAI._targetPlayer = null;
            enemyAI.CurrentState  = enemyAI.States["Wander"];
            return;
        }

        //플레이어가 Attack 범위 내에 있는지 확인해서 있으면 Attack 상태로 전환
        if (IsReadyToAttack && //공격 준비가 안되었다면 그냥 따라붙도록 함
            _targetDistance <= enemyAI._base.stats.attackRange)
        {
            enemyAI._targetPlayer = _targetPlayer;
            enemyAI.CurrentState  = enemyAI.States["Attack"];
            return;
        }

        //----------------------------------------------

        if (CliffDetect(enemyAI) || WallDetect(enemyAI))
            enemyAI._targetDirection *= -1; //절벽이나 벽 감지시 방향 반전

        //TODO : 너무 짧은 주기에 방향를 바꾸지 않도록 일정 시간 이후에 방향전환 가능하도록 수정
        enemyAI._targetDirection = enemyAI._transform.position.DirectionToX(enemyAI._targetPlayer.PlayerPosition);


        enemyAI._animator.SetBool("IsWalk", true);
        enemyAI._transform.position += enemyAI._targetDirection * (Time.deltaTime * enemyAI._base.stats.speed);
    }

    public override void Exit()
    {
    }
}

public class SAttack : State
{
    [SerializeField] private float _lastAttackTime;
    [SerializeField] private float _nextAttackTime;
    [SerializeField] private bool  _isAttacking;

    public override void Enter()
    {
        stateName = "Attack";
        enemyAI._animator.SetBool("IsAttack", true);
    }

    public override async void Execute()
    {
        if (_isAttacking) return;
        if (enemyAI.waitForAttack) AttackDelay();


        (Player _targetPlayer, float _targetDistance) = PlayerInRange(enemyAI);

        if (_targetDistance < 0)
        {
            enemyAI._targetPlayer = null;
            enemyAI.CurrentState  = enemyAI.States["Wander"];
            return;
        }

        if (enemyAI.waitForAttack || //공격 대기중이거나 너무 멀어졌으면 Chase 상태로 전환
            _targetDistance > enemyAI._base.stats.attackRange)
        {
            Debug.Log("Attack -> Chase");
            enemyAI.CurrentState = enemyAI.States["Chase"];
            return;
        }

        if (Time.time >= _nextAttackTime)
        {
            _lastAttackTime = Time.time;
            _nextAttackTime = _lastAttackTime + 1 / enemyAI._base.stats.attackSpeed; //공격속도에 따라 다음 공격시간 계산

            _isAttacking = true;
            await Attack();
            _isAttacking          = false;
            enemyAI.waitForAttack = true;
        }
    }

    public override void Exit()
    {
        Debug.Log("Attack Exit");
        enemyAI._animator.SetBool("IsAttack", false);
    }

    private async UniTask Attack()
    {
        enemyAI._animator.SetTrigger("Attack");

        var _attacked = Physics2D.OverlapCircleAll(enemyAI.transform.position, enemyAI._base.stats.attackRange, LayerMask.GetMask("Player")) ?? throw new ArgumentNullException("Physics2D.OverlapCircleAll(enemyAI.transform.position, enemyAI._base.stats.attackRange, LayerMask.GetMask(\"Player\"))");
        foreach (Collider2D _player in _attacked) _player.GetComponent<Player>().Attacked(enemyAI._base.stats.attackDamage, 0, enemyAI._base);

        Debug.DrawRay(enemyAI.transform.position, enemyAI.TowardPlayer * enemyAI._base.stats.attackRange, Color.red, 1f);

        int _time = enemyAI._animator.GetCurrentAnimatorClipInfo(0).Length;
        await UniTask.Delay(TimeSpan.FromSeconds(_time));
    }

    private async void AttackDelay()
    {
        Debug.Log("delay start");
        float _waitingTime = _nextAttackTime - Time.time;
        await UniTask.Delay(TimeSpan.FromSeconds(_waitingTime));
        enemyAI.waitForAttack = false;
        Debug.Log("delay end");
    }
}
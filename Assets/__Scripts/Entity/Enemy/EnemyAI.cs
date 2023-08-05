using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
enum EnemyAttackType
{
    Touch,
    Range
}

public class EnemyAI : MonoBehaviour
{
    private const float DAZED_DURATION = 0.35f;

    private const float CLIFF_DETECT_DISTANCE       = 0.5f;
    private const float CHANGE_DIRECTION_CYCLE_TIME = 1.0f;

    [Header("Set in Inspector")]
    [SerializeField] private bool canDazed = true;

    [SerializeField] private bool  canStun       = true;
    [SerializeField] private float chaseDistance = 5f;

    [SerializeField] private EnemyAttackType attackType     = EnemyAttackType.Range;
    [SerializeField] private float           attackRange    = 1f;
    [SerializeField] private bool            stopWhenAttack = true;

    private readonly WaitForSeconds _changeDirectionCycle = new WaitForSeconds(CHANGE_DIRECTION_CYCLE_TIME);
    private          EnemyBase      _base;

    private Player     _targetPlayer;
    private Collider2D _thisCollider;
    private Vector3    _colliderEdgeLeft, _colliderEdgeRight;

    private Vector3 _targetDirection;

    private Transform _transform;
    private Vector3   _targetPosition;

    public  Animator _animator;
    private bool     nowMove;

    //플레이어를 향한 벡터
    private Vector3 TowardPlayer => (_targetPosition - _transform.position).normalized;

    private bool  _canMove = true;
    private bool  _stunned,  _dazed;
    private float _stunTime, _dazeTime;

    public void Initialize(EnemyBase p_enemyBase)
    {
        _targetPlayer = GameManager.Instance.RandomPlayer();
        _thisCollider = GetComponent<Collider2D>();
        _base         = p_enemyBase;
        _transform    = transform;

        _animator                           = GetComponent<Animator>();
        _animator.runtimeAnimatorController = Animation.GetAnimatorController("Frost");

        bool playerInRange = CalcNextBehavior();
    }

    private bool CalcNextBehavior()
    {
        //일정 주기마다 플레이어 위치를 찾아서 해당 방향으로 이동
        if (!(Vector3.Distance(_targetPlayer.PlayerPosition, _transform.position) <= chaseDistance)) return false;

        _targetPosition = _targetPlayer.PlayerPosition;
        if (_targetPosition.x > _transform.position.x)
            _targetDirection = Vector3.right;
        else if (_targetPosition.x < _transform.position.x)
            _targetDirection = Vector3.left;

        return true;
    }

    public void Update()
    {
        if (_stunned)
            _stunTime -= Time.deltaTime;
        if (_dazed)
            _dazeTime -= Time.deltaTime;

        if (_stunTime <= 0) _stunned = false;
        if (_dazeTime <= 0) _dazed   = false;

        CalcAttack();

        var _bounds = _thisCollider.bounds;
        _colliderEdgeLeft  = _bounds.min;
        _colliderEdgeRight = _bounds.min + Vector3.right * _bounds.size.x;

        if (CLIFF_DETECT_DISTANCE > 0)
        {
            _targetDirection = CliffDetect() switch
            {
                1  => Vector3.left,
                -1 => Vector3.right,
                _  => _targetDirection
            };
        }

        if (_stunned || _dazed || !_canMove) return;

        //지금 animation의 state의 이름이 Attack으로 끝나면 이동하지 않음
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return;
        _transform.position += _targetDirection * (Time.deltaTime * _base.stats.speed);
        _animator.SetBool("IsWalk", true);
    }

#region CliffDetect

    private int CliffDetect()
    {
        return Physics2D.Raycast(_colliderEdgeLeft, Vector3.down, CLIFF_DETECT_DISTANCE, LayerMask.GetMask("Floor"))
                        .collider
                        .IsUnityNull()
            ? -1
            : Physics2D.Raycast(_colliderEdgeRight, Vector3.down, CLIFF_DETECT_DISTANCE, LayerMask.GetMask("Floor"))
                       .collider
                       .IsUnityNull()
                ? 1
                : 0;
    }

#endregion

    public void Stun(float p_stunDuration)
    {
        if (!canStun) return;
        _animator.SetBool("IsWalk", false);
        _stunned = true;
        if (!(_stunTime > p_stunDuration)) //기존 스턴시간이 더 길면 무시
            _stunTime = p_stunDuration;
    }

    public void Daze()
    {
        if (!canDazed) return;
        _animator.SetBool("IsWalk", false);
        _dazed    = true;
        _dazeTime = DAZED_DURATION;
    }

    private void CalcAttack()
    {
        if (Vector2.Distance(_targetPlayer.PlayerPosition, _transform.position) <= attackRange)
        {
            _canMove = false;
            _animator.SetBool("IsAttack", true);
            
            if (Time.time >= _nextAttackTime)
                Attack();
        }
        else
        {
            _animator.SetBool("IsAttack", false);
            _canMove = true;
        }
    }

    [SerializeField] private float _lastAttackTime;
    [SerializeField] private float _nextAttackTime;

    private void Attack()
    {
        _lastAttackTime = Time.time;
        _nextAttackTime = _lastAttackTime + 1 / _base.stats.attackSpeed; //공격속도에 따라 다음 공격시간 계산

        _animator.SetTrigger("Attack");
        Debug.Log("Attack!");
        Debug.DrawRay(transform.position, TowardPlayer * attackRange, Color.red, 1f);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private const float DAZED_DURATION        = 0.35f;
    private const float CLIFF_DETECT_DISTANCE = 0.5f;

    public EnemyBase _base;

    public Vector2 bodySize;

    public Player     _targetPlayer;
    public Collider2D _thisCollider;

    public Transform _transform;

    public Vector3 _targetDirection;
    public Vector3 _targetPosition;

    public  Animator _animator;
    private bool     nowMove;

    private bool  _canMove = true;
    private bool  _stunned,  _dazed;
    private float _stunTime, _dazeTime;

    public bool waitForAttack = false;

    //플레이어를 향한 벡터
    public Vector3 TowardPlayer => (_targetPosition - _transform.position).normalized;

    public State CurrentState
    {
        get => _currentState;
        set
        {
            _currentState?.Exit(); //현재상태 존재하면 Exit()호출
            _currentState = value; //현재상태를 새로운 상태로 변경
            _currentState.Enter(); //새로운 상태의 Enter()호출
        }
    }
    
    [Space]

    public Dictionary<string, State> States = new Dictionary<string, State>();

    [SerializeReference] private State  _currentState;
    
    public void Initialize(EnemyBase p_enemyBase)
    {
        _targetPlayer = GameManager.Instance.GetRandomPlayer();
        _thisCollider = GetComponent<Collider2D>();
        _base         = p_enemyBase;
        _transform    = transform;

        bodySize = _thisCollider.bounds.size;

        _animator                           = GetComponent<Animator>();
        _animator.runtimeAnimatorController = Animation.GetAnimatorController(_base.stats.enemyName);

        States.Add("Wander", new SWander().Initialize(this));
        States.Add("Chase",  new SChase().Initialize(this));
        States.Add("Attack", new SAttack().Initialize(this));

        CurrentState = States["Wander"];
    }

    public void Update()
    {
        if (_stunned)
        {
            _stunTime -= Time.deltaTime;
            _stunned  =  _stunTime > 0;
        }

        if (_dazed)
        {
            _dazeTime -= Time.deltaTime;
            _dazed    =  _dazeTime > 0;
        }

        if (_stunned || _dazed || !_canMove) return;

        CurrentState.Execute();

        //지금 animation의 state의 이름이 Attack으로 끝나면 이동하지 않음
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return;
        
        //_targetDirection은 언제나 단위벡터이지만, 혹시 모르니까 정규화 
        _transform.localScale = new Vector3(-_targetDirection.normalized.x, 1, 1);
    }

    public void Stun(float p_stunDuration)
    {
        if (!_base.stats.canStun) return;
        _animator.SetBool("IsWalk", false);
        _stunned = true;
        if (!(_stunTime > p_stunDuration)) //기존 스턴시간이 더 길면 무시
            _stunTime = p_stunDuration;
    }

    public void Daze()
    {
        if (!_base.stats.canDazed) return;
        _animator.SetBool("IsWalk", false);
        _dazed    = true;
        _dazeTime = DAZED_DURATION;
    }

    // private void CalcAttack()
    // {
    //     if (Vector2.Distance(_targetPlayer.PlayerPosition, _transform.position) <= _base.stats.attackRange)
    //     {
    //         _canMove = false;
    //         _animator.SetBool("IsAttack", true);
    //
    //         if (Time.time >= _nextAttackTime)
    //             Attack();
    //     }
    //     else
    //     {
    //         _animator.SetBool("IsAttack", false);
    //         _canMove = true;
    //     }
    // }



    
}
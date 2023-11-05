using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private const float DAZED_DURATION = 0.35f;

    public EnemyBase enemyBase;

    public Vector2 bodySize;

    public Player     targetPlayer;
    public Collider2D thisCollider;

    public Transform cachedTransform;

    public Vector3 targetDirection;
    public Vector3 targetPosition;

    public  Animator animator;
    private bool     _nowMove;

    private bool  _stunned,  _dazed;
    private float _stunTime, _dazeTime;

    public bool waitForAttack;

    //플레이어를 향한 벡터
    public Vector3 TowardPlayer => (targetPosition - cachedTransform.position).normalized;

    public State CurrentState
    {
        get => currentState;
        set
        {
            currentState?.Exit(); //현재상태 존재하면 Exit()호출
            currentState = value; //현재상태를 새로운 상태로 변경
            currentState.Enter(); //새로운 상태의 Enter()호출
        }
    }

    [Space]
    public Dictionary<string, State> States;

    [SerializeReference] private State currentState;

    private static readonly int IsAttacked = Animator.StringToHash("IsAttacked");
    private static readonly int IsWalk     = Animator.StringToHash("IsWalk");
    private static readonly int Attacked   = Animator.StringToHash("Attacked");

    public async void Initialize(EnemyBase _enemyBase)
    {
        thisCollider    = GetComponent<Collider2D>();
        enemyBase       = _enemyBase;
        cachedTransform = transform;

        bodySize = thisCollider.bounds.size;

        animator = enemyBase.animator;

        //재생중인 애니메이션이 끝날때 까지 대기
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

        States = new Dictionary<string, State>
                 {
                     { "Wander", new SWander().Initialize(this) },
                     { "Chase", new SChase().Initialize(this) },
                     { "Attack", new SAttack().Initialize(this) }
                 };

        CurrentState = States["Wander"];
    }

    public void Update()
    {
        if (States == null) return;

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

        if (_stunned || _dazed) return;
        animator.SetBool(IsAttacked, false);

        CurrentState?.Execute();

        //지금 animation의 state가 Attack이면 return
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return;

        FlipEntity();
    }

    private void FlipEntity() => cachedTransform.localScale = new Vector3(targetDirection.normalized.x, 1, 1);

    //_targetDirection은 언제나 단위벡터이지만, 혹시 모르니까 정규화 

    public void Stun(float _stunDuration)
    {
        if (!enemyBase.stats.canStun) return;
        animator.SetBool(IsWalk,     false);
        animator.SetBool(IsAttacked, true);
        animator.SetTrigger(Attacked);
        _stunned = true;
        if (!(_stunTime > _stunDuration)) //기존 스턴시간이 더 길면 무시
            _stunTime = _stunDuration;
    }

    public void Daze()
    {
        if (!enemyBase.stats.canDazed) return;
        animator.SetBool(IsWalk,     false);
        animator.SetBool(IsAttacked, true);
        animator.SetTrigger(Attacked);
        _dazed    = true;
        _dazeTime = DAZED_DURATION;
    }

    private bool  _explosionOnContact;
    private float _explosionStartTime;

    public void LaunchToPlayer()
    {
        animator.speed = 1;

        Rigidbody2D _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = targetPlayer.transform.position - transform.position + Vector3.up * 10;
        _explosionOnContact   = true;
        _explosionStartTime   = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_explosionOnContact) return;

        if (_other.CompareTag("Player"))
        {
            Debug.Log("PlayerExplosion");

            animator.SetBool("DoExplosion", true);
            animator.SetBool("IsWalk", false);
            enemyBase.Dead(null, false);
            currentState = null;
        }
        else if (_other.CompareTag("Floor") && Time.time - _explosionStartTime > 0.1f)
        {
            Debug.Log("FloorExplosion");
            animator.SetBool("DoExplosion", true);
            animator.SetBool("IsWalk",      false);
            currentState = null;
            StartCoroutine(ToDead());
        }
    }

    private IEnumerator ToDead()
    {
        Debug.Log("DoCoroutine");
        yield return new WaitForSeconds(0.1f);
        enemyBase.Dead(null, false);
    }
}
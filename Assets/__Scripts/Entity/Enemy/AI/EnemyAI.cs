using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private const float DAZED_DURATION        = 0.35f;

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

    public readonly Dictionary<string, State> States = new();

    [SerializeReference] private State  currentState;
    
    public void Initialize(EnemyBase p_enemyBase)
    {
        thisCollider = GetComponent<Collider2D>();
        enemyBase         = p_enemyBase;
        cachedTransform    = transform;

        bodySize = thisCollider.bounds.size;

        animator                           = GetComponent<Animator>();
        animator.runtimeAnimatorController = Animation.GetAnimatorController(enemyBase.stats.enemyName);

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

        if (_stunned || _dazed) return;

        CurrentState.Execute();

        //지금 animation의 state가 Attack이면 return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return;
        
        FlipEntity();

    }

    private void FlipEntity() => cachedTransform.localScale = new(-targetDirection.normalized.x, 1, 1);
    //_targetDirection은 언제나 단위벡터이지만, 혹시 모르니까 정규화 

    
    public void Stun(float p_stunDuration)
    {
        if (!enemyBase.stats.canStun) return;
        animator.SetBool("IsWalk", false);
        _stunned = true;
        if (!(_stunTime > p_stunDuration)) //기존 스턴시간이 더 길면 무시
            _stunTime = p_stunDuration;
    }

    public void Daze()
    {
        if (!enemyBase.stats.canDazed) return;
        animator.SetBool("IsWalk", false);
        _dazed    = true;
        _dazeTime = DAZED_DURATION;
    }



    
}
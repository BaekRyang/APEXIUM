using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public struct InputValues
{
    public float horizontal, vertical;
    public bool  jumpDown,   jumpUp;
}


public class PlayerController : MonoBehaviour
{
    private const float JUMP_GRACE_TIME           = 0.1f;
    private const float JUMP_BUFFER               = 0.1f;
    private const float DISABLE_LADDER_CLIMB_TIME = 0.2f;

    public Player player;
    
    private InputValues _input;

    private Rigidbody2D   _rigidbody2D;
    private BoxCollider2D _boxCollider2D;

    public readonly PlayerStats playerStats = new PlayerStats();

    private float Speed        => playerStats.Speed;
    private float JumpHeight   => playerStats.JumpHeight;
    private int   MaxJumpCount => playerStats.MaxJumpCount;


    private void Awake()
    {
        _rigidbody2D   = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void LoadSetting()
    {
        
    }


    private void Update()
    {
        if (player.clientID != GameManager.Instance.playerID)
            return;
        
        var _position = transform.position;
        jumpGraceTimer -= Time.deltaTime;
        jumpBuffer     -= Time.deltaTime;

        GetInput(); //Key Input 값 받아오기

        //점프중일때(상승) 바닥을 뚫고 올라갈 수 있게 해준다.
        _boxCollider2D.isTrigger = _rigidbody2D.velocity.y > 0 || climbLadder;
        //사다리를 타고있으면 중력 영향을 받지않게 해준다.
        _rigidbody2D.gravityScale = climbLadder ? 0 : 2;

        ClimbLadder(_position);

        Jump();

        ClampVelocity();

        SetZPosition();
    }

    private void FixedUpdate()
    {
        if (climbLadder) //사다리에 타고있으면 좌우 이동 막기
            return;

        Move();
    }


    private void OnCollisionEnter2D(Collision2D p_other)
    {
        if (p_other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (jumpBuffer > 0)
                Jump();
        }
    }

    private void OnCollisionExit(Collision p_other)
    {
    }

    private void OnTriggerEnter2D(Collider2D p_other)
    {
        if (p_other.gameObject.CompareTag("Ladder"))
        {
            onLadder  = true;
            ladderPos = p_other.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D p_other)
    {
        if (p_other.gameObject.CompareTag("Ladder"))
        {
            onLadder                   = false;
            climbLadder                = false;
            _rigidbody2D.gravityScale = 1;
            _boxCollider2D.isTrigger  = false;
        }
    }

#region Input

    private void GetInput()
    {
        _input = new InputValues
                  {
                      horizontal = Input.GetAxisRaw("Horizontal"),
                      vertical   = Input.GetAxisRaw("Vertical"),
                      jumpDown   = Input.GetButtonDown("Jump"),
                      jumpUp     = Input.GetButtonUp("Jump")
                  };
    }

#endregion

#region MoveAction

    private void Move()
    {
        _rigidbody2D.velocity = new Vector2(_input.horizontal * Speed, _rigidbody2D.velocity.y);
    }

#endregion

#region JumpAction

    [SerializeField] private int   jumpCount;
    [SerializeField] private float jumpBuffer;     //Buffering Time
    [SerializeField] private float jumpGraceTimer; //Coyote Time

    private void Jump()
    {
        if (!_input.jumpDown) return;

        //점프 버퍼 채워주고
        jumpBuffer = JUMP_BUFFER;

        //coyoteTimer가 0보다 크고 남은 점프횟수가 있다면 점프
        if (jumpGraceTimer > 0 && jumpCount < MaxJumpCount)
        {
            //사다리용 : 아래키 누르고 점프하면 위로 가속하지 않게 해준다.
            var _jumpHeight = _input.vertical < 0 ? -JumpHeight * .5f : JumpHeight;

            jumpBuffer     = 0;
            jumpGraceTimer = 0;
            jumpCount++;

            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpHeight);

            if (climbLadder)
                climbLadder = false;
        }
    }

#endregion

#region LadderAction

    [SerializeField] private Vector3 ladderPos;
    [SerializeField] private bool    onLadder;
    [SerializeField] private bool    climbLadder;

    private void ClimbLadder(Vector3 p_position)
    {
        if (climbLadder && _input.vertical == 0)
            _rigidbody2D.velocity = Vector2.zero;

        //상하 움직임이 없거나, 사다리를 타고 있을 때
        if (Mathf.Abs(_rigidbody2D.velocity.y) < 0.001f || climbLadder)
        {
            //코요테 타임 및 점프 카운트 초기화 
            jumpGraceTimer = JUMP_GRACE_TIME;
            jumpCount      = 0;
        }


        if (_input.vertical == 0) return;

        //처음 사다리에 타는 액션 : 사다리에 닿아있지만 타는중은 아닐때 그리고 "점프한지 0.2초 이내일때"
        //사다리에서 위나 아래키를 누르면서 점프를 할때 사다리에 다시 타지 않도록 DISABLE_LADDER_CLIMB_TIME초의 딜레이를 준다.
        if (onLadder && !climbLadder && jumpBuffer <= -DISABLE_LADDER_CLIMB_TIME)
        {
            if (_input.vertical > 0 &&
                !Physics2D.Raycast(transform.position, Vector2.down, .1f, LayerMask.GetMask("Floor")).collider
                         .IsUnityNull())
                //사다리 위에서 윗 키를 눌렀을때 사다리를 타지 않도록 해준다.
                return;

            climbLadder            = true;
            transform.position     = new Vector3(ladderPos.x, p_position.y, ladderPos.z - .1f); //사다리에 붙여주고
            _rigidbody2D.velocity = Vector2.zero;                                            //가속 초기화
        }

        if (climbLadder) //타고있는중에는 상하 이동만 해준다.
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, Input.GetAxis("Vertical") * Speed);
    }

#endregion

#region Gravity

    [SerializeField] private float maxFallSpeed = 10;

    private void ClampVelocity()
    {
        //하강속도 제한
        if (_rigidbody2D.velocity.y < -maxFallSpeed)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -maxFallSpeed);
    }

#endregion

#region zPositionSetting

    private void SetZPosition()
    {
        if (jumpBuffer > -0.2f || climbLadder) return;

        var _transform = transform;
        var _position   = _transform.position;

        var _raycast  = Physics2D.Raycast(_position, Vector2.down, 1f, LayerMask.GetMask("Floor"));
        Debug.DrawRay(_position, Vector2.down, Color.red);
        if (!_raycast.collider.IsUnityNull())
        {
            _transform.position =
                new Vector3(transform.position.x, _position.y, _raycast.transform.position.z);
        }
    }

#endregion
}
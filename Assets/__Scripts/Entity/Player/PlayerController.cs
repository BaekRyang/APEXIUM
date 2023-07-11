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
        // if (player.clientID != GameManager.Instance.playerID)
        //     return;
        //멀티 플레이어 환경

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

        ResetJump();
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
            onLadder                  = false;
            climbLadder               = false;
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

        // _input = new InputValues
        //          {
        //              horizontal = VJoystick.MovementDirection,
        //              vertical   = Input.GetAxisRaw("Vertical"),
        //              jumpDown   = Input.GetButtonDown("Jump"),
        //              jumpUp     = Input.GetButtonUp("Jump")
        //          };
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

    public int _jumpDir;

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

            _jumpDir = _input.horizontal == 0 ? 0 : _input.horizontal > 0 ? 1 : -1;

            if (climbLadder)
                climbLadder = false;
        }
    }

#endregion

#region ResetJump

    private void ResetJump()
    {
        //상하 움직임이 없거나, 사다리를 타고 있을 때
        if (Mathf.Abs(_rigidbody2D.velocity.y) < 0.001f || climbLadder)
        {
            //코요테 타임 및 점프 카운트 초기화 
            jumpGraceTimer = JUMP_GRACE_TIME;
            jumpCount      = 0;
            _jumpDir       = -2;
        }
    }

#endregion

#region LadderAction

    [SerializeField] private Vector2 ladderPos;
    [SerializeField] private bool    onLadder;
    [SerializeField] private bool    climbLadder;

    public int _nowDir;
    private void ClimbLadder(Vector3 p_position)
    {
        //사다리를 타고있으면서, 상하 이동을 하지 않을때 velocity를 0으로
        if (climbLadder && _input.vertical == 0)
            _rigidbody2D.velocity = Vector2.zero;

        //상하이동이 없거나 사다리를 타고있지 않으면 리턴
        if (_input.vertical == 0 || !onLadder) return;

        //만약 _jumpDir이랑 내가 가고있는 방향이 같다면 사다리를 타지 않는다.
        //즉 내릴 마음으로 점프를 했다면 사다리를 타지 않는다.
        _nowDir = _input.horizontal == 0 ? 0 : _input.horizontal > 0 ? 1 : -1;
        if (_jumpDir == _nowDir)
            return;
        
        Bounds  _collider = _boxCollider2D.bounds;
        Vector2 _mdPoint  = new Vector2(_collider.center.x, _collider.min.y);
        if (_input.vertical > 0 &&
            !Physics2D.Raycast(_mdPoint, Vector2.down, .1f, LayerMask.GetMask("Floor")).collider.IsUnityNull())
            //사다리 위에서 윗 키를 눌렀을때 사다리를 타지 않도록 해준다.
            return;

        climbLadder           = true;
        transform.position    = new Vector3(ladderPos.x, p_position.y); //사다리에 붙여주고
        _rigidbody2D.velocity = Vector2.zero;                           //가속 초기화

        //사다리에서 나갈때 틩기는 현상을 막기위해 velocity사용을 하지 않게 변경
        // if (climbLadder) //타고있는중에는 상하 이동만 해준다.
        //     _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, Input.GetAxis("Vertical") * Speed);

        if (climbLadder)
        {
            float deltaY = _input.vertical * Speed * Time.deltaTime;
            transform.position += new Vector3(0, deltaY, 0);
        }
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
}
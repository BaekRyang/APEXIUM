using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

[Serializable]
public struct InputValues
{
    public float horizontal,   vertical;
    public bool  jumpDown,     jumpUp;
    public bool  primarySkill, secondarySkill, movementSkill, ultimateSkill;
    public bool  specialSkill, itemSkill;
}

public class PlayerController : MonoBehaviour
{
    private const float JUMP_GRACE_TIME = 0.2f;
    private const float JUMP_BUFFER     = 0.2f;

    public Tilemap _ladderTilemap;

    public Player player;

    public InputValues _input;

    private Rigidbody2D   _rigidbody2D;
    private BoxCollider2D _boxCollider2D;

    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    public PlayerStats playerStats;

    private int      _jumpCountOffset; //점프 카운트를 조정하기 위한 변수
    public  JumpDire _jumpDirection;
    public  JumpDire _lastLadderJumpDirection;

    private float Speed        => playerStats.speed;
    private float JumpHeight   => playerStats.JumpHeight;
    private int   MaxJumpCount => playerStats.MaxJumpCount + _jumpCountOffset;

    public Facing PlayerFacing => transform.localScale.x > 0 ? Facing.Left : Facing.Right;

    public bool Controllable { get; private set; } = true;

    public enum Facing
    {
        Left  = -1,
        Right = 1
    }

    private void Awake()
    {
        _rigidbody2D   = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _ladderTilemap = GameObject.Find("Grid").transform.Find("Ladder").GetComponent<Tilemap>();

        GameManager.Instance.virtualCamera.Follow = transform; //수정 필요 (GameManager보다 먼저 Awake되는 경우가 있을 수 있음)
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
        _rigidbody2D.gravityScale = climbLadder ? 0 : 3;

        //공중점프 제한 및 코요테타임 구현용
        //코요테 타임은 바닥에 붙어있을때 언제나 일정 수준을 유지하므로
        //jumpGraceTimer가 0이면 공중에 떠있는 상태이므로, 최대 점프 횟수를 -1 조정해준다.
        _jumpCountOffset = jumpGraceTimer <= 0 ? -1 : 0;

        SetLadderStatus();
        ClimbLadder(_position);

        Jump();

        ClampVelocity();

        ResetJump();

        _jumpDirection = GetNowJumpDirection();

        UseSkill();
    }

    private void FixedUpdate()
    {
        if (climbLadder) //사다리에 타고있으면 좌우 이동 막기
            return;

        Move();
    }

    //바닥 착지시 실행할 Action
    private Action _landingAction;

    public void AddLandingAction(Action p_action)
    {
        _landingAction += p_action;
    }

    private void OnCollisionEnter2D(Collision2D p_other)
    {
        if (p_other.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (jumpBuffer > 0) //바닥에 닿았는데 찰나의 차이로 점프를 미리 시도했다면 점프를 실행해준다.
            {
                Debug.Log("Buffered Jump");
                jumpCount = 0;
                Jump(true);
            }

            //landingAction 실행후 초기화(바닥에 착지시 실행할 Action)
            _landingAction?.Invoke();
            _landingAction = null;
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
                     jumpUp     = Input.GetButtonUp("Jump"),

                     primarySkill   = Input.GetButton("PrimarySkill"),
                     secondarySkill = Input.GetButton("SecondarySkill"),
                     movementSkill  = Input.GetButton("MovementSkill"),
                     ultimateSkill  = Input.GetButton("UltimateSkill"),
                     specialSkill   = Input.GetButton("SpecialSkill"),
                     itemSkill      = Input.GetButton("ItemSkill")
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
        if (!Controllable) return;

        _rigidbody2D.velocity = new Vector2(_input.horizontal * Speed, _rigidbody2D.velocity.y);
        player._animator.SetBool("IsWalk", _input.horizontal != 0);
        FlipSprite();
    }

    //이동 방향에 따라서 스프라이트를 뒤집어준다.
    private void FlipSprite()
    {
        Transform _transformCache = transform;

        _transformCache.localScale = _input.horizontal switch
        {
            > 0 => new Vector3(-1, 1, 1),
            < 0 => new Vector3(1,  1, 1),
            _   => _transformCache.localScale
        };
    }

#endregion

#region JumpAction

    [SerializeField] private int   jumpCount;
    [SerializeField] private float jumpBuffer;     //Buffering Time
    [SerializeField] private float jumpGraceTimer; //Coyote Time

    private void Jump(bool p_forced = false)
    {
        if (!p_forced) //점프키를 무시하는 강제점프가 아니라면
            if (!_input.jumpDown) return; //점프키 상태 확인
        
        if (!Controllable) return;

        //점프키가 눌러졌으므로, 점프 버퍼는 채워준다.
        jumpBuffer = JUMP_BUFFER;

        //최대 점프 횟수에 도달하면 점프하지 않는다.
        if (jumpCount >= MaxJumpCount)
        {
            Debug.Log($"Max Jump Count - {jumpCount} / {MaxJumpCount}");
            return;
        }

        //사다리에서 아래방향 점프를 했다면 아래 방향으로 떨어뜨려준다.
        //=>내리려는 의도로 점프를 했을때 작동
        var _jumpHeight = climbLadder && _input.vertical < 0 ? -JumpHeight * .5f : JumpHeight;

        jumpBuffer = 0; //점프 했으므로 버퍼 초기화
        jumpCount++;    //점프횟수 증가

        //사다리를 타고있었다면 사다리 타기를 종료한다.
        if (climbLadder)
        {
            player._animator.speed = 1;
            climbLadder            = false;
            player._animator.SetBool("IsClimb", false);
        }

        //실제 점프 및 점프관련 애니메이션 실행
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpHeight);
        Animation.PlayAnimation(player._animator, "Jump");
        player._animator.SetBool("IsJump", true);

        if (jumpGraceTimer > 0)              //정상적으로 점프를 했다면
            jumpGraceTimer = float.MaxValue; //점프 횟수 조정을 받지 않도록 최대값을 넣어준다. (0이하가 되면 점프 횟수가 줄어듦)
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
            _jumpDirection = _lastLadderJumpDirection = JumpDire.None;
            player._animator.SetBool("IsJump", false);
        }
    }

#endregion

#region LadderAction

    [SerializeField] private Vector2 ladderPos;
    [SerializeField] private bool    onLadder;
    [SerializeField] private bool    climbLadder;

    [Serializable]
    public enum JumpDire
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    private void ClimbLadder(Vector3 p_position)
    {
        //사다리를 타고있으면서, 상하 이동을 하지 않을때 velocity를 0으로
        if (climbLadder && _input.vertical == 0)
        {
            _rigidbody2D.velocity  = Vector2.zero;
            player._animator.speed = 0;
        }

        //상하이동이 없거나 사다리를 타고있지 않으면 리턴
        if (_input.vertical == 0 || !onLadder) return;

        //만약 _jumpDir이랑 내가 가고있는 방향이 같다면 사다리를 타지 않는다.
        //즉 내릴 마음으로 점프를 했다면 사다리를 타지 않는다.
        if (climbLadder && _input.jumpDown)
            _lastLadderJumpDirection = GetNowJumpDirection();

        if (_jumpDirection == _lastLadderJumpDirection) //플레이어가 내리기 위해서 점프했음을 감지하여
            return;                                     //사다리에 다시 붙지 못하도록 한다.

        Bounds  _bounds  = _boxCollider2D.bounds;
        Vector2 _mdPoint = new Vector2(_bounds.center.x, _bounds.min.y);
        if (_input.vertical > 0 &&
            !climbLadder        &&
            !Physics2D.Raycast(_mdPoint, Vector2.down, .1f, LayerMask.GetMask("Floor")).collider.IsUnityNull())

            //사다리 위에서 윗 키를 눌렀을때 사다리를 타지 않도록 해준다.
        {
            Debug.Log("Disable Ladder Jump");
            return;
        }

        player._animator.SetBool("IsClimb", true);


        climbLadder           = true;
        transform.position    = new Vector3(ladderPos.x, p_position.y); //사다리에 붙여주고
        _rigidbody2D.velocity = Vector2.zero;                           //가속 초기화

        //사다리에서 나갈때 틩기는 현상을 막기위해 velocity사용을 하지 않게 변경
        // if (climbLadder) //타고있는중에는 상하 이동만 해준다.
        //     _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, Input.GetAxis("Vertical") * Speed);

        if (climbLadder)
        {
            float _deltaY = _input.vertical * Speed * Time.deltaTime;
            transform.position += new Vector3(0, _deltaY, 0);
        }

        player._animator.speed = 1;
    }

    private JumpDire GetNowJumpDirection()
    {
        return _input switch
        {
            { horizontal: > 0 }              => JumpDire.Right,
            { horizontal: < 0 }              => JumpDire.Left,
            { horizontal: 0, vertical: > 0 } => JumpDire.Up,
            { horizontal: 0, vertical: < 0 } => JumpDire.Down,
            _                                => JumpDire.None
        };
    }

    /// <summary>
    /// Player status for ladder.
    /// </summary>
    private void SetLadderStatus()
    {
        //현재 서있는 곳이 사다리인지 확인한다. (아래쪽으로 가는 경우에는 한칸 아래를 조사한다.)
        if (HasLadderTile(_input.vertical < 0))
            onLadder = true;
        else
        {
            if (!onLadder) return;
            onLadder                  = false;
            climbLadder               = false;
            _rigidbody2D.gravityScale = 1;
            _boxCollider2D.isTrigger  = false;
            player._animator.SetBool("IsClimb", false);
        }
    }

    /// <summary>
    /// Check if there is a ladder tile at the player position.
    /// </summary>
    /// <param name="p_isDown">Check one tile down.</param>
    /// <returns>True if there is a ladder tile.</returns>
    private bool HasLadderTile(bool p_isDown = false)
    {
        
        Vector3Int _tilePosition = new Vector3Int(Mathf.FloorToInt(transform.position.x),
                                                  Mathf.FloorToInt(transform.position.y - (p_isDown ? .1f : 0))); //오차보정
        if (_ladderTilemap.HasTile(_tilePosition))
        {
            ladderPos = new Vector2(_tilePosition.x + .5f, _tilePosition.y);
            return true;
        }

        return false;
    }

#endregion

#region Gravity

    [SerializeField] private float maxFallSpeed = 20;

    private void ClampVelocity()
    {
        //하강속도 제한
        if (_rigidbody2D.velocity.y < -maxFallSpeed)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -maxFallSpeed);
    }

#endregion

#region SkillUse

    private void UseSkill()
    {
        if (_input.itemSkill) //아이템 스킬은 언제나 사용가능
            player.skills[SkillTypes.Item].Play();

        if (climbLadder) return;

        //이외는 사다리에서 사용 불가능

        if (_input.primarySkill)
            player.skills[SkillTypes.Primary].Play();
        else if (_input.secondarySkill)
            player.skills[SkillTypes.Secondary].Play();
        else if (_input.movementSkill)
            player.skills[SkillTypes.Utility].Play();
        else if (_input.ultimateSkill)
            player.skills[SkillTypes.Ultimate].Play();
        else if (_input.specialSkill)
            player.skills[SkillTypes.Passive].Play();
    }

#endregion

    public void SetControllable(bool p_pControllable)
    {
        //플레이어의 조작 가능 여부를 설정한다.
        Controllable = p_pControllable;
        if (!p_pControllable && _rigidbody2D.velocity.y == 0) //공중이 아니라면 x속도도 0으로 만들어준다.
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
    }
}
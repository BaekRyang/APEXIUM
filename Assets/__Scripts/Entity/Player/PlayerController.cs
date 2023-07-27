using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public struct InputValues
{
    public float horizontal,   vertical;
    public bool  jumpDown,     jumpUp;
    public bool  primarySkill, secondarySkill, movementSkill, ultimateSkill;
    public bool  specialSkill, itemSkill;
}

public class PlayerController : MonoBehaviour
{
    private const float JUMP_GRACE_TIME           = 0.1f;
    private const float JUMP_BUFFER               = 0.1f;
    private const float DISABLE_LADDER_CLIMB_TIME = 0.2f;

    public Tilemap _ladderTilemap;

    public Player player;

    private InputValues _input;

    private Rigidbody2D   _rigidbody2D;
    private BoxCollider2D _boxCollider2D;

    public PlayerStats playerStats = new PlayerStats();

    private int      _jumpCountOffset;
    public  JumpDire _jumpDirection;
    public  JumpDire _lastLadderJumpDirection;

    private float Speed        => playerStats.speed;
    private float JumpHeight   => playerStats.jumpHeight;
    private int   MaxJumpCount => playerStats.maxJumpCount + _jumpCountOffset;
    
    public Facing PlayerFacing => transform.localScale.x > 0 ? Facing.Left : Facing.Right;
    
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

        GameManager.Instance.virtualCamera.Follow = transform;
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

    // private void OnTriggerEnter2D(Collider2D p_other)
    // {
    //     if (p_other.gameObject.CompareTag("Ladder"))
    //     {
    //         onLadder  = true;
    //         ladderPos = p_other.transform.position;
    //     }
    // }
    //
    // private void OnTriggerExit2D(Collider2D p_other)
    // {
    //     if (p_other.gameObject.CompareTag("Ladder"))
    //     {
    //         onLadder                  = false;
    //         climbLadder               = false;
    //         _rigidbody2D.gravityScale = 1;
    //         _boxCollider2D.isTrigger  = false;
    //     }
    // }

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
        _rigidbody2D.velocity = new Vector2(_input.horizontal * Speed, _rigidbody2D.velocity.y);
        FlipSprite();
    }
    
    //이동 방향에 따라서 스프라이트를 뒤집어준다.
    private void FlipSprite()
    {
        Transform _transformCache = transform;
        
        _transformCache.localScale = _input.horizontal switch
        {
            > 0 => new Vector3(-1, 1, 1),
            < 0 => new Vector3(1, 1, 1),
            _   => _transformCache.localScale
        };
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
        if (jumpCount < MaxJumpCount)
        {
            //사다리용 : 아래키 누르고 점프하면 위로 가속하지 않게 해준다.
            var _jumpHeight = climbLadder && _input.vertical < 0 ? -JumpHeight * .5f : JumpHeight;
            jumpBuffer = 0;
            jumpCount++;
            GameManager.Instance._cellSlider.value = MaxJumpCount - jumpCount;

            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpHeight);

            if (climbLadder)
                climbLadder = false;

            if (jumpGraceTimer > 0)
                jumpGraceTimer = 100;
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
            jumpGraceTimer                         = JUMP_GRACE_TIME;
            jumpCount                              = 0;
            GameManager.Instance._cellSlider.value = MaxJumpCount;
            _jumpDirection                         = _lastLadderJumpDirection = JumpDire.None;
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
            _rigidbody2D.velocity = Vector2.zero;

        //상하이동이 없거나 사다리를 타고있지 않으면 리턴
        if (_input.vertical == 0 || !onLadder) return;

        //만약 _jumpDir이랑 내가 가고있는 방향이 같다면 사다리를 타지 않는다.
        //즉 내릴 마음으로 점프를 했다면 사다리를 타지 않는다.
        if (climbLadder && _input.jumpDown)
            _lastLadderJumpDirection = GetNowJumpDirection();

        if (_jumpDirection == _lastLadderJumpDirection) //플레이어가 내리기 위해서 점프했음을 감지하여
            return;                                     //사다리에 다시 붙지 못하도록 한다.

        Bounds  _bounds = _boxCollider2D.bounds;
        Vector2 _mdPoint  = new Vector2(_bounds.center.x, _bounds.min.y);
        if (_input.vertical > 0 &&
            !climbLadder        &&
            !Physics2D.Raycast(_mdPoint, Vector2.down, .1f, LayerMask.GetMask("Floor")).collider.IsUnityNull())

            //사다리 위에서 윗 키를 눌렀을때 사다리를 타지 않도록 해준다.
        {
            Debug.Log("Disable Ladder Jump");
            return;
        }

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
                                                 Mathf.FloorToInt(transform.position.y - (p_isDown ? 1 : 0)));

        if (_ladderTilemap.HasTile(_tilePosition))
        {
            ladderPos = new Vector2(_tilePosition.x + .5f, _tilePosition.y);
            return true;
        }

        return false;
    }

#endregion

#region Gravity

    [SerializeField] private float maxFallSpeed = 15;

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
        if (_input.primarySkill)
        {
            player.skills[0].Play(1);
            Debug.Log("PrimarySkill");
        } else if (_input.secondarySkill)
        {
            player.skills[1].Play(1);
            Debug.Log("SecondarySkill");
        } else if (_input.movementSkill)
        {
            player.skills[2].Play(1);
            Debug.Log("MovementSkill");
        } else if (_input.ultimateSkill)
        {
            player.skills[3].Play(1);
            Debug.Log("UltimateSkill");
        } else if (_input.specialSkill)
        {
            player.skills[4].Play(1);
            Debug.Log("SpecialSkill");
        } else if (_input.itemSkill)
        {
            player.skills[5].Play(1);
            Debug.Log("ItemSkill");
        }
    }
    

#endregion
}
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private const float DAZED_DURATION = 0.35f;

    private const float CLIFF_DETECT_DISTANCE       = 0.5f;
    private const float CHANGE_DIRECTION_CYCLE_TIME = 1.0f;

    [Header("Set in Inspector")]
    [SerializeField] private bool canDazed = true;

    [SerializeField] private bool canStun = true;

    private readonly WaitForSeconds _changeDirectionCycle = new WaitForSeconds(CHANGE_DIRECTION_CYCLE_TIME);

    [SerializeField] private Player targetPlayer;

    [SerializeField] private Collider2D thisCollider;
    private                  Vector3    _colliderEdgeLeft, _colliderEdgeRight;

    private Vector3 _targetDirection;

    [SerializeField] private float     chaseDistance = 5f;
    private                  Transform _transform;
    private                  Vector3   _targetPosition;

    private MonsterStats _stats;

    private bool  _stunned,  _dazed;
    private float _stunTime, _dazeTime;

    public void Initialize(EnemyBase p_enemyBase)
    {
        targetPlayer = GameManager.Instance.RandomPlayer();
        thisCollider = GetComponent<Collider2D>();
        _stats       = p_enemyBase.stats;
        _transform   = transform;

        StartCoroutine(CalcNextBehavior());
    }

    private IEnumerator CalcNextBehavior()
    {
        while (true)
        {
            //일정 주기마다 플레이어 위치를 찾아서 해당 방향으로 이동
            yield return _changeDirectionCycle;
            if (!(Vector3.Distance(targetPlayer.PlayerPosition, _transform.position) <= chaseDistance)) continue;
            _targetPosition = targetPlayer.PlayerPosition;

            if (_targetPosition.x > _transform.position.x)
                _targetDirection = Vector3.right;
            else if (_targetPosition.x < _transform.position.x)
                _targetDirection = Vector3.left;
        }
    }

    public void Update()
    {
        if (_stunned)
            _stunTime -= Time.deltaTime;
        if (_dazed)
            _dazeTime -= Time.deltaTime;

        if (_stunTime <= 0) _stunned = false;
        if (_dazeTime <= 0) _dazed   = false;
        
        


        var _bounds = thisCollider.bounds;
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

        if (_stunned || _dazed) return;
        _transform.position += _targetDirection * (Time.deltaTime * _stats.speed);
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

        _stunned = true;
        if (!(_stunTime > p_stunDuration)) //기존 스턴시간이 더 길면 무시
            _stunTime = p_stunDuration;
    }

    public void Daze()
    {
        if (!canDazed) return;
        
        _dazed    = true;
        _dazeTime = DAZED_DURATION;
    }
}
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private const float CLIFF_DETECT_DISTANCE       = 0.5f;
    private const float CHANGE_DIRECTION_CYCLE_TIME = 1.0f;

    private readonly WaitForSeconds _changeDirectionCycle = new WaitForSeconds(CHANGE_DIRECTION_CYCLE_TIME);

    [SerializeField] private Player targetPlayer;

    [SerializeField] private Collider2D thisCollider;
    private                  Vector3    _colliderEdgeLeft, _colliderEdgeRight;

    private Vector3 _targetDirection;

    [SerializeField] private float     chaseDistance = 5f;
    private                  Transform _transform;
    private                  Vector3   _targetPosition;

    private Stats _stats;

    private bool _stunned;

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
        var bounds = thisCollider.bounds;
        _colliderEdgeLeft  = bounds.min;
        _colliderEdgeRight = bounds.min + Vector3.right * bounds.size.x;

        if (CLIFF_DETECT_DISTANCE > 0)
        {
            _targetDirection = CliffDetect() switch
            {
                1  => Vector3.left,
                -1 => Vector3.right,
                _  => _targetDirection
            };
        }

        if (_stunned) return;
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
}
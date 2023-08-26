using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType type;
    [SerializeField] private int        value;
    [SerializeField] private bool       interactable;
    public                   Vector2    _targetPosition;
    public                   Vector2    _randomDirection;

    public void Initialize(PickupType p_type, PickupSize p_pickupSize)
    {
        type  = p_type;
        value = (int)p_pickupSize;

        InitializeMove();
    }

    private async void InitializeMove()
    {
        Debug.Log("InitializeMove");
        switch (type)
        {
            case PickupType.Resource:
                Rigidbody2D _rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
                _rigidbody2D.freezeRotation = true;
                _rigidbody2D.gravityScale   = 2;
                _rigidbody2D.angularDrag    = Random.Range(1f, 2f);

                _rigidbody2D.sharedMaterial = new PhysicsMaterial2D
                                              {
                                                  bounciness = Random.Range(0.5f, .8f),
                                                  friction   = Random.Range(0.2f, .4f)
                                              };

                
                BoxCollider2D _collider = gameObject.AddComponent<BoxCollider2D>();
                _collider.excludeLayers = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Pickup");

                
                //위쪽 방향으로 랜덤한 힘을 가함
                _rigidbody2D.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f)) * 5f, ForceMode2D.Impulse);
                await UniTask.Delay(1000);
                break;

            case PickupType.Exp:
                await SpreadObject();
                await UniTask.Delay(1000);
                break;

            case PickupType.Health:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        interactable = true;
    }

    private async UniTask SpreadObject()
    {
        //3의 범위 안의 랜덤한 방향을 구한다.
        _randomDirection = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        Vector2 _originPosition = transform.position;
        _targetPosition = _originPosition + _randomDirection;

        //해당 방향으로 0.2초동안 이동한다.
        //TODO: duration은 다른곳에 옮기는것이 좋을듯
        float _elapsedTime = 0;
        float _duration    = .5f;
        while (_elapsedTime < _duration)
        {
            transform.position =  Vector2.Lerp(_originPosition, _targetPosition, EaseOut(_elapsedTime / _duration));
            _elapsedTime        += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    private void OnTriggerEnter2D(Collider2D p_other)
    {
        if (!interactable && !p_other.CompareTag("Player")) return;
        if (!p_other.TryGetComponent(out Player _player)) return;

        Destroy(gameObject);

        switch (type)
        {
            case PickupType.Resource:
                _player.Stats.CommonResource += value;
                break;
            case PickupType.Exp:
                _player.Stats.Exp += value;
                break;
            case PickupType.Health:
                _player.Stats.Health += value;
                break;
        }
    }

    
    //TODO: 임시로 만들었음(옮기거나 삭제)
    public static float Linear(float t) //선형보간
    {
        return t;
    }

    public static float EaseOut(float t)
    {
        return Mathf.Sin(Mathf.Pow(t, 0.5f) * Mathf.PI / 2);
    }

    public static float EaseIn(float t)
    {
        return 1 - Mathf.Cos((t * Mathf.PI) / 2);
    }

    public static float EaseInOut(float t)
    {
        return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1);
    }
}
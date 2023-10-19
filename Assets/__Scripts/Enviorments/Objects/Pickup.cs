using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pickup : MonoBehaviour
{
    [SerializeField] public  PickupType  pickupType;
    [SerializeField] private int         value;
    [SerializeField] private bool        interactable;
    public                   Vector2     targetPosition;
    public                   Vector2     randomDirection;
    public                   Rigidbody2D _rigidbody2D;
    public                   float       _attractForceInSecond;

    public int PickupValue
    {
        get => value;
        set => this.value = value;
    }

    public async void Activate(Player _player)
    {
        if (pickupType == PickupType.Resource)
            _rigidbody2D.gravityScale = 2;

        interactable = false;

        await ActivateMove();

        interactable = true;

        StartCoroutine(AttractToPlayer(_player));
    }

    private IEnumerator AttractToPlayer(Player _player)
    {
        if (pickupType is PickupType.Item or PickupType.Health ||
            _player is null)
            yield break;

        if (pickupType is PickupType.Resource)
            SetRigidbodyState(false);

        //플레이어 방향으로 끌려간다.

        yield return new WaitForSeconds(2f);

        //초당 끌려가는 힘
        _attractForceInSecond = 0f;
        float _t = 0;

        while (true)
        {
            if (!gameObject.activeSelf) 
                yield break;

            if (_rigidbody2D != null) 
                _rigidbody2D.gravityScale = 0;

            Vector3 _toPlayerVector = _player.transform.position - transform.position;
            Vector3 _nextPosition   = _toPlayerVector.normalized * (_attractForceInSecond * Time.deltaTime);
            
            _t                     += Time.deltaTime;
            _attractForceInSecond =  _t * _t;

            if (_toPlayerVector.magnitude < _nextPosition.magnitude)
                _nextPosition = _nextPosition.normalized * _toPlayerVector.magnitude;

            transform.position += _nextPosition;
            yield return null;
        }
    }

    public void SetRigidbodyState(bool _enable)
    {
        _rigidbody2D.velocity      = Vector2.zero;
        _rigidbody2D.excludeLayers = _enable ? 0 : LayerMask.GetMask("Floor");
        _rigidbody2D.gravityScale  = _enable ? 2 : 0;
    }

    private async UniTask ActivateMove()
    {
        switch (pickupType)
        {
            case PickupType.Item:
                await FloatingMove();
                break;

            case PickupType.Resource:
                //위쪽 방향으로 랜덤한 힘을 가함
                _rigidbody2D.AddForce(new Vector2(Random.Range(-.7f, .7f), Random.Range(1f, 2f)) * 7.5f, ForceMode2D.Impulse);
                await UniTask.Delay(1500);
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
    }

    private const float FLOATING_DISTANCE = 1;
    private const float FLOATING_DURATION = .5f;

    private async UniTask FloatingMove()
    {
        Vector2 _originPosition = transform.position;
        targetPosition = _originPosition + new Vector2(0, FLOATING_DISTANCE);

        MMTweenType _tween = new(MMTween.MMTweenCurve.EaseOutQuadratic);

        float _elapsedTime = 0;
        while (_elapsedTime < FLOATING_DURATION)
        {
            float _t = _tween.Evaluate(_elapsedTime / FLOATING_DURATION);
            transform.position =  Vector2.Lerp(_originPosition, targetPosition, _t);
            _elapsedTime       += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    private async UniTask SpreadObject()
    {
        //3의 범위 안의 랜덤한 방향을 구한다.
        randomDirection = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        Vector2 _originPosition = transform.position;
        targetPosition = _originPosition + randomDirection;

        MMTweenType _tween = new(MMTween.MMTweenCurve.EaseOutQuadratic);

        //해당 방향으로 0.2초동안 이동한다.
        float _elapsedTime = 0;
        float _duration    = 1f; //TODO: duration은 다른곳에 옮기는것이 좋을듯
        while (_elapsedTime < _duration)
        {
            float _t = _tween.Evaluate(_elapsedTime / _duration);
            transform.position =  Vector2.Lerp(_originPosition, targetPosition, _t);
            _elapsedTime       += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!interactable) return;

        if (_other.CompareTag("Player"))
        {
            if (!_other.TryGetComponent(out Player _player)) return;

            switch (pickupType)
            {
                case PickupType.Item:
                    _player.items.AddItem(value);
                    break;
                case PickupType.Resource:
                    _player.Stats.EnergyCrystal += value;
                    break;
                case PickupType.Exp:
                    _player.Stats.Exp += value;
                    break;
                case PickupType.Health:
                    _player.Stats.Health += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            interactable = false;
            gameObject.SetActive(false);
        }
        else if (_other.CompareTag("PlayerPickRadius") &&
                 pickupType == PickupType.Resource)
            _rigidbody2D.gravityScale = 0;
    }

    private void OnTriggerStay2D(Collider2D _other)
    {
        if (!interactable || pickupType == PickupType.Item) return;
        if (!_other.CompareTag("PlayerPickRadius")) return;

        //닿아있으면 플레이어 방향으로 끌려간다.
        // transform.Translate((p_other.transform.position - transform.position).normalized * Time.deltaTime * 5f);
        transform.position += (_other.transform.position - transform.position).normalized * Time.deltaTime * 5f;
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (!interactable) return;

        if (!_other.CompareTag("PlayerPickRadius")) return;

        if (pickupType == PickupType.Resource)
            _rigidbody2D.gravityScale = 2;
    }
}
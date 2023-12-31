using System;
using TMPro;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] protected TMP_Text text;

    protected bool destroyAfterInteract;

    private void Start()
    {
        text         = GetComponentInChildren<TMP_Text>();
        text.enabled = false;

        text.transform.position = new Vector2(transform.position.x, GetComponent<Collider2D>().bounds.max.y);

        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
            text.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
            text.enabled = false;
    }

    public void Interact(Player _player = null)
    {
        if (!InteractPredicate(_player))
        {
            CanNotInteractAction();
            return;
        }

        GetComponent<SpriteAnimation>()?.Play();

        InteractAction(_player);

        if (destroyAfterInteract)
            DestroyAction();
    }

    protected abstract void CanNotInteractAction();
    protected abstract bool InteractPredicate(Player _player);

    private void DestroyAction()
    {
        DestroyImmediate(text.gameObject);
        DestroyImmediate(this);
    }

    protected abstract void InteractAction(Player _player);

    //오브젝트에 상호작용했을때 실행되는 함수

    protected abstract void Initialize();

    //오브젝트가 생성될때 실행되는 함수
}

public class ItemSpawnEvent
{
    public readonly PickupType pickupType;
    public readonly int        value;
    public readonly Vector3    position;
    public readonly Player     player;

    public ItemSpawnEvent(PickupType _pickupType, int _value, Vector3 _position, Player _player)
    {
        pickupType = _pickupType;
        value      = _value;
        position   = _position;
        player     = _player;
    }
}
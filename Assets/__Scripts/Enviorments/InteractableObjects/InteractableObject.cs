using System;
using TMPro;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text;

    protected bool destroyAfterInteract;

    private void Start()
    {
        text         = GetComponentInChildren<TMP_Text>();
        text.enabled = false;
        animator     = GetComponent<Animator>();

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
        if (animator != null)
            animator.Play("Interact");

        InteractAction(_player);

        if (destroyAfterInteract)
            DestroyAction();
    }

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
    public readonly int        amount;
    public readonly Vector3    position;

    public ItemSpawnEvent(PickupType _pickupType, int _amount, Vector3 _position)
    {
        pickupType = _pickupType;
        amount     = _amount;
        position   = _position;
    }
}
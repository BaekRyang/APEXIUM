using System;
using TMPro;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    private const            float    INTERACT_DELAY = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text;
    [SerializeField] private bool     destroyAfterInteract;

    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        text.enabled = false;
        animator = GetComponent<Animator>();
        
        text.transform.position = new Vector2(transform.position.x, GetComponent<Collider2D>().bounds.max.y);
    }

    private void OnTriggerEnter2D(Collider2D p_other)
    {
        if (p_other.CompareTag("Player"))
            text.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D p_other)
    {
        if (p_other.CompareTag("Player"))
            text.enabled = false;
    }

    public void Interact()
    {
        if (animator != null) 
            animator.Play("Interact");

        InteractAction();

        if (!destroyAfterInteract) return;
        DestroyImmediate(text.gameObject);
        DestroyImmediate(this);
    }

    protected abstract void InteractAction();
    //오브젝트에 상호작용했을때 실행되는 함수
}

public class ItemSpawnEvent
{
    public PickupType pickupType;
    public int        amount;
    public Vector3    position;

    public ItemSpawnEvent(PickupType _pickupType, int _amount, Vector3 _position)
    {
        pickupType = _pickupType;
        amount     = _amount;
        position   = _position;
    }
}
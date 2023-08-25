using System;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text;
    
    private bool _closeToInteract;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
            _closeToInteract = text.enabled = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _closeToInteract = text.enabled = false;
    }

    private void Update()
    {
        if (_closeToInteract && Input.GetKeyDown(KeyCode.A))
            Interact();
    }

    public virtual void Interact()
    {
        animator.Play("Interact");
        DestroyImmediate(text.gameObject);
        DestroyImmediate(this);
    }
}

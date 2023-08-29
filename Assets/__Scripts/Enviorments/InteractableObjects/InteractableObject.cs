using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text;

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
        animator.Play("Interact");
        
        InteractAction();
        
        DestroyImmediate(text.gameObject);
        DestroyImmediate(this);
    }

    protected abstract void InteractAction();
    //오브젝트에 상호작용했을때 실행되는 함수
}
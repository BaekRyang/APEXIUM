using UnityEngine;

public class JumpingSpot : MonoBehaviour
{
    public float jumpForce = 10f;
    
    private void OnTriggerEnter2D(Collider2D p_other)
    {
        Debug.LogError("CHANGE JUMP ATTRIBUTE");
        if (p_other.TryGetComponent(out IEntity _))
            p_other.attachedRigidbody.velocity = Vector2.up * jumpForce;
    }
}
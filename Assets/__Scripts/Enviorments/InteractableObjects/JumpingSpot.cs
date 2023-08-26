using UnityEngine;

public class JumpingSpot : MonoBehaviour
{
    public float jumpForce = 10f;
    
    private void OnTriggerEnter2D(Collider2D p_other)
    {
        if (p_other.attachedRigidbody != null)
            p_other.attachedRigidbody.velocity = Vector2.up * jumpForce;
    }
}
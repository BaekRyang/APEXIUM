using UnityEngine;

public class JumpingSpot : MonoBehaviour
{
    public float jumpForce = 10f;
    
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.TryGetComponent(out IEntity _))
            _other.attachedRigidbody.velocity = Vector2.up * jumpForce;
    }
}
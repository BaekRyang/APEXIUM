using System;
using Unity.VisualScripting;
using UnityEngine;

public enum PickupType
{
    Resource,
    Exp,
    Health
}

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType type;
    [SerializeField] private int        value;

    private void OnTriggerEnter2D(Collider2D p_other)
    {
        if (!p_other.CompareTag("Player")) return;
        if (!p_other.TryGetComponent(out Player _player)) return;

        Destroy(gameObject);

        switch (type)
        {
            case PickupType.Resource:
                this.AddComponent<Rigidbody2D>(); //동전은 바닥에 떨어지게
                _player.Stats.Resource += value;
                break;
            case PickupType.Exp:
                _player.Stats.Exp += value;
                break;
            case PickupType.Health:
                _player.Stats.Health += value;
                break;
        }
    }
}
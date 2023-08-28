using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PickupType
{
    Resource,
    Exp,
    Health
}

public enum PickupSize
{
    Large  = 150,
    Medium = 25,
    Small  = 1
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private void Start() => Instance ??= this;

    public void InstantiatePickupObjects(PickupType p_pickupType, int p_value, Transform p_transform)
    {
        if (p_value <= 0) return;

        int _large  = (p_value)                         / (int)PickupSize.Large;
        int _medium = (p_value % (int)PickupSize.Large) / (int)PickupSize.Medium;
        int _small  = (p_value % (int)PickupSize.Large) % (int)PickupSize.Medium;


        foreach ((PickupSize _pickupSize, int _pickupValue) in new[] { (PickupSize.Large, _large), (PickupSize.Medium, _medium), (PickupSize.Small, _small) })
        {
            foreach (Pickup _pickup in PickupPool.Instance.GetAvailablePickupComponents(p_pickupType, _pickupValue))
                PickupInitialize(_pickup, _pickupSize);
        }

        return;
    
        void PickupInitialize(Pickup p_pickup, PickupSize p_size)
        {
            p_pickup.PickupValue          = (int)p_size;
            p_pickup.transform.position   = p_transform.position + Vector3.up;
            p_pickup.transform.localScale = Vector3.one * GetPickupSize(p_size);
            p_pickup.gameObject.SetActive(true);
            p_pickup.InitializeMove();
        }
    }

    private float GetPickupSize(PickupSize p_pickupSize) => p_pickupSize switch
    {
        PickupSize.Large  => 2f,
        PickupSize.Medium => 1.5f,
        PickupSize.Small  => 1f,
        _                 => 1f
    };
}
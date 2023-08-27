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

    [SerializeField] private Sprite     exp, health, resource;
    [SerializeField] private GameObject pickupPrefab;

    public void InstantiatePickupObjects(PickupType p_pickupType, int p_value, Transform p_transform)
    {
        if (p_value <= 0) return;

        int _large  = (p_value)                         / (int)PickupSize.Large;
        int _medium = (p_value % (int)PickupSize.Large) / (int)PickupSize.Medium;
        int _small  = (p_value % (int)PickupSize.Large) % (int)PickupSize.Medium;


        PickupPool.Instance.GetAvailablePickupComponents(p_pickupType, _large).ForEach(p_pickup =>
        {
            p_pickup.PickupValue = (int)PickupSize.Large;
            p_pickup.transform.position = p_transform.position + Vector3.up;
            p_pickup.transform.localScale *= GetPickupSize(PickupSize.Large);
            p_pickup.gameObject.SetActive(true);
            p_pickup.InitializeMove();
        });
        PickupPool.Instance.GetAvailablePickupComponents(p_pickupType, _medium).ForEach(p_pickup =>
        {
            p_pickup.PickupValue = (int)PickupSize.Medium;
            p_pickup.transform.position = p_transform.position + Vector3.up;
            p_pickup.transform.localScale *= GetPickupSize(PickupSize.Large);
            p_pickup.gameObject.SetActive(true);
            p_pickup.InitializeMove();
        });
        PickupPool.Instance.GetAvailablePickupComponents(p_pickupType, _small).ForEach(p_pickup =>
        {
            p_pickup.PickupValue = (int)PickupSize.Small;
            p_pickup.transform.position = p_transform.position + Vector3.up;
            p_pickup.transform.localScale *= GetPickupSize(PickupSize.Large);
            p_pickup.gameObject.SetActive(true);
            p_pickup.InitializeMove();
        });
        
        // for (int _i = 0; _i < _large; _i++) InstantiatePickupObject(p_pickupType,  PickupSize.Large,  p_transform);
        // for (int _i = 0; _i < _medium; _i++) InstantiatePickupObject(p_pickupType, PickupSize.Medium, p_transform);
        // for (int _i = 0; _i < _small; _i++) InstantiatePickupObject(p_pickupType,  PickupSize.Small,  p_transform);
    }


    private float GetPickupSize(PickupSize p_pickupSize) => p_pickupSize switch
    {
        PickupSize.Large  => 2f,
        PickupSize.Medium => 1.5f,
        PickupSize.Small  => 1f,
        _                 => 1f
    };
}
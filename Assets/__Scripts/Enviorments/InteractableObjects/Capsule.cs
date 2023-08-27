using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Capsule : InteractableObject
{
    [SerializeField] private int _expAmount;
    [SerializeField] private int _resourceAmount;

    public bool open;

    private void LateUpdate()
    {
        if (open) Interact();
    }

    protected override void InteractAction()
    {
        ItemManager.Instance.InstantiatePickupObjects(PickupType.Exp, _expAmount, transform);
        ItemManager.Instance.InstantiatePickupObjects(PickupType.Resource, _resourceAmount, transform);

    }
}
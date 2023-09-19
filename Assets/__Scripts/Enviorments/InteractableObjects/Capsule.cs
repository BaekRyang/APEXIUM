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
        if (_expAmount > 0) EventBus.Publish(new ItemSpawnEvent(PickupType.Exp, _expAmount, transform.position));
        
        if (_resourceAmount > 0) EventBus.Publish(new ItemSpawnEvent(PickupType.Resource, _resourceAmount, transform.position));
    }
}
using UnityEngine;

public class ItemBox : InteractableObject
{
    [SerializeField] private int itemID = -1;
    [SerializeField] private int expAmount;
    [SerializeField] private int resourceAmount;

    public bool open;

    private void LateUpdate()
    {
        if (open) Interact();
    }

    protected override void InteractAction(Player _player)
    {
        if (itemID != -1) 
            EventBus.Publish(new ItemSpawnEvent(PickupType.Item, itemID, transform.position));

        if (expAmount > 0) 
            EventBus.Publish(new ItemSpawnEvent(PickupType.Exp, expAmount, transform.position));
        
        if (resourceAmount > 0) 
            EventBus.Publish(new ItemSpawnEvent(PickupType.Resource, resourceAmount, transform.position));
    }

    protected override void Initialize()
    {
        destroyAfterInteract = true;
    }
}
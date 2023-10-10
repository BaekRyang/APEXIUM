using UnityEngine;
using UnityEngine.Localization.PropertyVariants;

public class ItemBox : InteractableObject
{
    [SerializeField] private int expAmount;
    [SerializeField] private int resourceAmount;

    [SerializeField] public int requiredResourceAmount = -1;

    public bool IsItemChest => requiredResourceAmount > 0;

    [SerializeField] public ChestType chestType;

    public   bool open;

    private void LateUpdate()
    {
        //TODO : 삭제
        if (open) Interact();
    }

    protected override bool InteractPredicate(Player _player)
    {
        return requiredResourceAmount <= 0 ||
               _player.ConsumeResource(requiredResourceAmount);
    }

    protected override void InteractAction(Player _player)
    {
        if (IsItemChest)
        {
            int _itemID = GetRandomItem(chestType);
            EventBus.Publish(new ItemSpawnEvent(PickupType.Item, _itemID, transform.position));
        }

        if (expAmount > 0)
            EventBus.Publish(new ItemSpawnEvent(PickupType.Exp, expAmount, transform.position));

        if (resourceAmount > 0)
            EventBus.Publish(new ItemSpawnEvent(PickupType.Resource, resourceAmount, transform.position));
    }

    private int GetRandomItem(ChestType _chestType)
    {
        return 0;
    }

    protected override void Initialize()
    {
        destroyAfterInteract = true;

        requiredResourceAmount = GameManager.GetRandomChestCost(chestType);
    }
}
using MoreMountains.Feedbacks;
using UnityEngine;

public class ItemBox : InteractableObject
{
    [SerializeField] private int expAmount;
    [SerializeField] private int resourceAmount;

    [SerializeField] public int requiredResourceAmount = -1;

    public bool IsItemChest => requiredResourceAmount > 0;

    [SerializeField] public ChestType chestType;

    public bool open;

    private MMF_Player _player;

    private void LateUpdate()
    {
        //TODO : 삭제
        if (open) Interact();
    }

    protected override void CanNotInteractAction()
    {
        _player ??= GetComponent<MMF_Player>();
        _player.PlayFeedbacks();
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
            EventBus.Publish(new ItemSpawnEvent(PickupType.Item, _itemID, transform.position, _player));
            return;
        }

        if (expAmount > 0)
            EventBus.Publish(new ItemSpawnEvent(PickupType.Exp, expAmount, transform.position, _player));

        if (resourceAmount > 0)
            EventBus.Publish(new ItemSpawnEvent(PickupType.Resource, resourceAmount, transform.position, _player));
    }

    private int GetRandomItem(ChestType _chestType)
    {
        Debug.Log($"<color=red>GetRandomItem</color> : {GameManager.GetRarityFromChestType(_chestType)}");
        
        
        
        // return Random.Range(0, 100);
        return 0;
    }
    

    protected override void Initialize()
    {
        destroyAfterInteract = true;
        
        requiredResourceAmount = GameManager.GetRandomChestCost(chestType);

        if (IsItemChest) return;
        
        expAmount      = GameManager.GetRandomCapsuleReward(PickupType.Exp);
        resourceAmount = GameManager.GetRandomCapsuleReward(PickupType.Resource);
    }
}
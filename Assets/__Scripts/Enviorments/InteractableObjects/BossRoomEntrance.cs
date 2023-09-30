using UnityEngine;

public class BossRoomEntrance : InteractableObject
{
    protected override void Initialize()
    {
        destroyAfterInteract = false;
    }
    
    protected override void InteractAction()
    {
        Debug.Log("BossRoomEntrance InteractAction");
    }
}

using UnityEngine;

public class GlobalInstaller :MonoBehaviour
{
    public GameObject playerPrefab;
    public Client client;
    public void Awake()
    {
        DIContainer.Global.Regist(playerPrefab, "PlayerPrefab");
        DIContainer.Global.Regist(new PlayerManager().Initialize());
        DIContainer.Global.Regist(client);
    }
    
}
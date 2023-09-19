using UnityEngine;

public class ProjectInstaller :MonoBehaviour
{

    public GameObject playerPrefab;
    public Client client;
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public void Awake()
    {
        var platerManager = new PlayerManager();
        
        DIContainer.Global.Regist(playerPrefab, "PlayerPrefab");
        DIContainer.Global.Regist(platerManager);
        platerManager.Initialize();
        DIContainer.Global.Regist(client);

        
    }
    
}
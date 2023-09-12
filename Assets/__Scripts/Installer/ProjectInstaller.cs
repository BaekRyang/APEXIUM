using UnityEngine;

public class ProjectInstaller :MonoBehaviour
{

    public GameObject playerPrefab;
    public Client client;
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public void Awake()
    {
        DIContainer.Global.Regist(new PlayerManager());
        DIContainer.Inject(client);
        
        DIContainer.Global.Regist(client);
        DIContainer.Global.Regist(playerPrefab, "PlayerPrefab");
        
    }
    
}
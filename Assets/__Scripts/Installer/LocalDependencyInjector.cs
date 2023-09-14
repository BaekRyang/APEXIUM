using System;
using UnityEngine;

public class LocalDependencyInjector : MonoBehaviour
{
    public GameManager GameManager;
    
    private void Awake()
    {
        DIContainer.Local = new DIContainer();
        
        DIContainer.Local.Regist(GameManager);

    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] public int              clientID;
    private                 PlayerController _playerController;
    private                 PlayerStats      _stats;
    
    public Vector3 PlayerPosition => transform.position;

    private IEnumerator LoadSettings()
    {
        _playerController        = gameObject.AddComponent<PlayerController>();
        _playerController.player = this;
        _stats                   = _playerController.playerStats;
        
        yield break;
    }
    
    void Start()
    {
        StartCoroutine(LoadSettings());
    }

    void Update()
    {
        
    }
}

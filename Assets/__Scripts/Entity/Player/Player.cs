using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] public int              clientID;
    private                 PlayerController _playerController;
    private                 PlayerStats      _stats;
    public                  Vector3          PlayerPosition => transform.position;
    
    public readonly List<IAttackable> skills = new List<IAttackable>();

    public PlayerController Controller => _playerController;

    public PlayerStats Stats => _stats;

    private IEnumerator LoadSettings()
    {
        _playerController = gameObject.AddComponent<PlayerController>();
        Controller.player = this;
        _stats            = Controller.playerStats;
        
        skills.Add(gameObject.AddComponent<RevolverShot>());
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
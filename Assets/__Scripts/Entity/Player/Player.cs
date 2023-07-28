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

    public readonly Dictionary<SkillTypes, Skill> skills = new Dictionary<SkillTypes, Skill>();

    public PlayerController Controller => _playerController;

    public PlayerStats Stats => _stats;

    private IEnumerator LoadSettings()
    {
        _stats = new PlayerStats()
                 .SetHealth(100)
                 .SetAttackDamage(10)
                 .SetSpeed(4f)
                 .SetDefense(0)
                 .SetAttackSpeed(1f)
                 .SetMaxJumpCount(1)
                 .SetJumpHeight(10);


        _playerController      = gameObject.AddComponent<PlayerController>();
        Controller.player      = this;
        Controller.playerStats = _stats;

        skills.Add(SkillTypes.Primary,   gameObject.AddComponent<RevolverShot>());
        skills.Add(SkillTypes.Secondary, gameObject.AddComponent<PierceShot>());
        skills.Add(SkillTypes.Utility,   gameObject.AddComponent<Roll>());
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public int clientID;

    private PlayerController _playerController;
    private PlayerStats      _stats;
    private bool             _isImmune;
    public  Vector3          PlayerPosition => transform.position;

    public readonly Dictionary<SkillTypes, Skill> skills = new();

    private MMF_Player       _statusFeedback;
    private MMF_FloatingText _floatingText;

    public Animator         _animator;
    public PlayerController Controller => _playerController;

    public PlayerStats Stats => _stats;

    public bool dead;

    private void LoadSettings(PlayerData p_playerData)
    {
        _stats = new(p_playerData.stats);
        UIElements.Instance.SetHealth(_stats.Health, _stats.MaxHealth);

        _playerController      = gameObject.AddComponent<PlayerController>();
        _animator              = GetComponent<Animator>();
        Controller.player      = this;
        Controller.playerStats = _stats;

        _animator.runtimeAnimatorController = Animation.GetAnimatorController(p_playerData.characterName);

        skills.Add(SkillTypes.Passive,   SkillFactory.MakeSkill(p_playerData.skillPassive,   this));
        skills.Add(SkillTypes.Primary,   SkillFactory.MakeSkill(p_playerData.skillPrimary,   this));
        skills.Add(SkillTypes.Secondary, SkillFactory.MakeSkill(p_playerData.skillSecondary, this));
        skills.Add(SkillTypes.Utility,   SkillFactory.MakeSkill(p_playerData.skillUtility,   this));
        skills.Add(SkillTypes.Ultimate,  SkillFactory.MakeSkill(p_playerData.skillUltimate,  this));

        _statusFeedback = GetComponentInChildren<MMF_Player>();
        _floatingText   = _statusFeedback.GetFeedbackOfType<MMF_FloatingText>();

        Collider2D _collider2D = GetComponent<Collider2D>();
        _collider2D.sharedMaterial.friction = 0f;

        _collider2D.enabled = false;
        _collider2D.enabled = true;
    }

    void Start()
    {
        LoadSettings(GameManager.Instance.GetCharacterData("Astro"));
    }

    private void Update()
    {
        foreach ((SkillTypes _, Skill _skill) in skills) 
            _skill.Update();
    }

    public void PlayStatusFeedback(string p_pText)
    {
        if (_statusFeedback.IsUnityNull())
        {
            Debug.Log("NULL");
            return;
        }

        _floatingText.Value = p_pText;
        _statusFeedback.PlayFeedbacks();
    }

    public void Attacked(int p_pDamage, float p_stunDuration, EnemyBase p_attacker)
    {
        if (_isImmune || dead) return;

        PlayStatusFeedback(p_pDamage.ToString());
        bool _stillAlive = HealthChange(-p_pDamage);

        if (_stillAlive) return; //아래는 죽었을때 이벤트 처리
        Die(p_attacker);
    }

    private bool HealthChange(int p_pDamage)
    {
        Stats.Health += p_pDamage;
        if (clientID == GameManager.Instance.playerID) //해당 캐릭터가 자신의 캐릭터일때만 UI 업데이트
            UIElements.Instance.SetHealth(Stats.Health, Stats.MaxHealth);

        return Stats.Health > 0; //체력이 0이하면 false 반환
    }

    private void Die(EnemyBase p_attacker)
    {
        dead = true;                       //죽었음을 표시
        Controller.SetControllable(false); //이동 불가
        _animator.SetTrigger("Dead");      //꼭 필요한지는 모르겠음 (체크 필요)
        _animator.enabled = false;         //애니메이션을 수동으로 조작하기 위해서 비활성화'


        Collider2D _collider2D = GetComponent<Collider2D>();

        _collider2D.sharedMaterial.friction = 1f;

        //이렇게 안하면 바로 적용이 안됨
        _collider2D.enabled = false;
        _collider2D.enabled = true;


        Controller.Rigidbody2D.velocity = Vector2.zero; //속도 초기화


        //마지막으로 공격한 몬스터 -> 플레이어 방향으로 밀어낸다.
        Vector2 _direction = new((transform.position - p_attacker.transform.position).normalized.x, 2f);

        //해당 x방향이 +이면 SR의 x Flip을 true 아니면 false
        GetComponent<SpriteRenderer>().flipX = _direction.x > 0;

        Controller.Rigidbody2D.AddForce(_direction * 7f, ForceMode2D.Impulse);
        StartCoroutine(DeadAction());
    }

    private IEnumerator DeadAction()
    {
        AnimationClip _deadClip = _animator.runtimeAnimatorController.animationClips.FirstOrDefault(_clip => _clip.name.EndsWith("Dead"));

        //이름이 Dead로 끝나는 클립을 찾아 저장한다. 애니메이션 종류가 많지 않으므로 LINQ를 사용해도 Performance 손실이 적음

        if (_deadClip == null) yield break; //FirstOrDefault는 값을 찾지 못하면 null 반환하므로 null체크

        while (true)
        {
            if (Controller.Rigidbody2D.velocity.sqrMagnitude != 0) //아직 날아가는 중 이라면
                _deadClip.SampleAnimation(gameObject, 0);          //1번 스프라이트
            else
            {
                _deadClip.SampleAnimation(gameObject, 0.025f); //바닥에 떨어졌다면 2번 스프라이트
                yield return new WaitForSeconds(1f);
                _deadClip.SampleAnimation(gameObject, 0.05f); //1초뒤 마지막 스프라이트 (죽은 상태)
                yield break;
            }

            yield return null;
        }
    }
}
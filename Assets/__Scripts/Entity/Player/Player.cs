using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Player : MonoBehaviour, IEntity
{
    [Inject] private PlayerManager _playerManager;
    [Inject] private CameraManager _cameraManager;

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

    [SerializeField] public Items items;

    public                   bool       dead;
    private static readonly  int        IsDead = Animator.StringToHash("IsDead");

    private void Initialize(PlayerData _playerData)
    {
        _stats = new PlayerStats(_playerData.stats)
                 {
                     isLocalPlayer = true
                 };

        items = new Items(this);

        _playerController      = gameObject.AddComponent<PlayerController>().Initialize(this);
        Controller.playerStats = _stats;

        _animator = GetComponent<Animator>();

        GetComponent<SpriteRenderer>().sprite = _playerData.sprite;
        _animator.runtimeAnimatorController   = _playerData.animatorController;

        skills.Add(SkillTypes.Item,      SkillFactory.MakeSkill("ItemSkill",                this));
        skills.Add(SkillTypes.Passive,   SkillFactory.MakeSkill(_playerData.skillPassive,   this));
        skills.Add(SkillTypes.Primary,   SkillFactory.MakeSkill(_playerData.skillPrimary,   this));
        skills.Add(SkillTypes.Secondary, SkillFactory.MakeSkill(_playerData.skillSecondary, this));
        skills.Add(SkillTypes.Utility,   SkillFactory.MakeSkill(_playerData.skillUtility,   this));
        skills.Add(SkillTypes.Ultimate,  SkillFactory.MakeSkill(_playerData.skillUltimate,  this));

        _statusFeedback = GetComponentInChildren<MMF_Player>();
        _floatingText   = _statusFeedback.GetFeedbackOfType<MMF_FloatingText>();

        Collider2D _collider2D = GetComponent<Collider2D>();
        _collider2D.sharedMaterial.friction = 0f;

        _collider2D.enabled = false;
        _collider2D.enabled = true;

        if (_playerManager.IsLocalPlayer(clientID))
            _cameraManager.SetCameraFollow(transform);

        if (TryGetComponent(out PlayerInput _playerInput))
            _playerInput.uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
    }

    void Start()
    {
        DIContainer.Inject(this);
        Initialize(DataManager.GetCharacterData("Astro"));
    }

    private void Update()
    {
        foreach ((SkillTypes _, Skill _skill) in skills)
            _skill.Update();
    }

    public void PlayStatusFeedback(string _text)
    {
        if (_statusFeedback.IsUnityNull())
        {
            Debug.Log("NULL");
            return;
        }

        _floatingText.Value = _text;
        _statusFeedback.PlayFeedbacks();
    }

    public void Attacked(int _damage, float _stunDuration, EnemyBase _attacker)
    {
        if (_isImmune || dead) return;

        PlayStatusFeedback(_damage.ToString());
        bool _stillAlive = HealthChange(-_damage);

        if (_stillAlive) return; //아래는 죽었을때 이벤트 처리
        Die(_attacker);
    }

    private bool HealthChange(int _damage)
    {
        Stats.Health += _damage;

        return Stats.Health > 0; //체력이 0이하면 false 반환
    }

    private void Die(EnemyBase _attacker)
    {
        dead = true;                       //죽었음을 표시
        Controller.SetControllable(false); //이동 불가

        Collider2D _collider2D = GetComponent<Collider2D>();

        _collider2D.sharedMaterial.friction = 1f;

        //이렇게 안하면 바로 적용이 안됨
        _collider2D.enabled = false;
        _collider2D.enabled = true;

        Controller.Rigidbody2D.velocity = Vector2.zero; //속도 초기화

        //마지막으로 공격한 몬스터 -> 플레이어 방향으로 밀어낸다.
        Vector2 _direction = new((transform.position - _attacker.transform.position).normalized.x, 2f);

        Controller.Rigidbody2D.AddForce(_direction * 7f, ForceMode2D.Impulse);
        DeadAction();
    }

    private async void DeadAction()
    {
        AnimationClip _deadClip = _animator.runtimeAnimatorController.animationClips.FirstOrDefault(_clip => _clip.name.EndsWith("Dead"));
        Time.timeScale = .3f;

        //이름이 Dead로 끝나는 클립을 찾아 저장한다. 애니메이션 종류가 많지 않으므로 LINQ를 사용해도 Performance 손실이 적음

        if (_deadClip == null) return; //FirstOrDefault는 값을 찾지 못하면 null 반환하므로 null체크

        _animator.SetBool(IsDead, true);

        //0.3초에 걸쳐서 TimeScale을 1으로 만든다.(Lerp)
        float       _elapsedTime = 0;
        float       _duration    = 3.5f;
        MMTweenType _tween       = new(MMTween.MMTweenCurve.EaseInQuartic);

        while (_elapsedTime < _duration)
        {
            float _t = _tween.Evaluate(_elapsedTime / _duration);
            Time.timeScale =  Mathf.Lerp(0.1f, 1f, _t);
            _elapsedTime   += Time.unscaledDeltaTime;
            await UniTask.Yield();
        }

        while (true)
        {
            if (Controller.Rigidbody2D.velocity.sqrMagnitude <= 0.01f)
            {
                _animator.speed = 1;
                break;
            }

            await UniTask.Yield();
        }
    }

    private void StopAnimation()
    {
        Debug.Log("STOP");
        _animator.speed = 0;
    }

    public bool ConsumeResource(int _requiredResourceAmount)
    {
        if (Stats.EnergyCrystal < _requiredResourceAmount) return false;

        Stats.EnergyCrystal -= _requiredResourceAmount;
        return true;
    }
}
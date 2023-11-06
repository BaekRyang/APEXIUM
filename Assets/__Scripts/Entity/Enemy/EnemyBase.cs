using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IEntity
{
    [SerializeField] private BoxCollider2D     _attackCollider;
    [SerializeField] private Vector4           _attackColliderOffsets;
    [Inject]                 ObjectPoolManager _objectPoolManager;

    private const float VERTICAL_OFFSET    = .7f;
    private const float CRITICAL_FONT_SIZE = 1.05f;

    private EnemyAI          _enemyAI;
    private MMF_Player       _damageFeedback;
    private MMF_FloatingText _floatingText;

    public EnemyStats stats;
    public Animator   animator;

    [SerializeField] private EnemyData enemyData;

    public event EventHandler OnEnemyHpChange;

    readonly InjectObj _injectObj = new();

    private void Start()
    {
        if (enemyData == null)
            return;

        Initialize(enemyData);
    }

    public void Initialize(EnemyData _importedData)
    {
        enemyData = _importedData;
        _injectObj.CheckAndInject(this);
        stats = new EnemyStats(enemyData);

        GetComponent<SpriteRenderer>().sprite = enemyData.sprite;

        if (!TryGetComponent(out animator))
            gameObject.AddComponent<Animator>();

        animator.runtimeAnimatorController = enemyData.animatorController;

        if (enemyData.isBoss)
            BossHealthDisplay.Instance.SyncToBossHealthBar(this);

        if (!TryGetComponent(out _enemyAI))
            _enemyAI = transform.AddComponent<EnemyAI>();

        GetComponent<Rigidbody2D>().simulated = true;

        _attackColliderOffsets = enemyData.attackColliderOffsets;

        if (TryGetComponent(out PolygonCollider2D _polygonCollider2D))
        {
            Destroy(_polygonCollider2D);
            gameObject.AddComponent<PolygonCollider2D>();
        }

        _attackCollider.offset = new Vector2(_attackColliderOffsets.x, _attackColliderOffsets.y);
        _attackCollider.size   = new Vector2(_attackColliderOffsets.z, _attackColliderOffsets.w);

        _enemyAI.Initialize(this);
        _damageFeedback = transform.Find("DamageFeedback").GetComponent<MMF_Player>();
        _damageFeedback.Initialization();

        _floatingText = _damageFeedback.GetFeedbackOfType<MMF_FloatingText>();

        DifficultyManager.OnDifficultyChange += LevelUp;
    }

    private readonly        Dictionary<uint, uint> _attackID = new();
    private static readonly int                    IsDead    = Animator.StringToHash("IsDead");

    public void Attacked(int _damage, bool _isCritical, float _stunDuration, Player _attacker, uint? _nowAttackID = null)
    {
        _floatingText.Value = _damage.ConvertDamageUnit(_isCritical, Tools.DamageUnitType.Full);

        //크리티컬은 빨강 아니면 하양
        _floatingText.Intensity = _isCritical ? CRITICAL_FONT_SIZE : 1;

        if (!_nowAttackID.HasValue) //ID가 없으면 기본 위치에
            _floatingText.TargetPosition = transform.position;
        else //ID가 있으면
        {
            if (_attackID.ContainsKey(_nowAttackID.Value)) //해당 ID의 공격이 이미 있으면
                _attackID[_nowAttackID.Value]++;           //해당 아이디의 value를 증가
            else                                           //없으면
                _attackID.Add(_nowAttackID.Value, 0);      //해당 ID의 공격을 만들고 value를 초기화

            _floatingText.TargetPosition = transform.position + Vector3.up * (VERTICAL_OFFSET * _attackID[_nowAttackID.Value]);
        }

        //여기서 오류나면 Exception 처리만 해주면 됨
        _damageFeedback.PlayFeedbacks();

        if (_stunDuration == 0)
            _enemyAI.Daze();
        else
            _enemyAI.Stun(_stunDuration);

        GetDamage(_damage);

        Knockback(_attacker, .15f);

        if (stats.Health <= 0)
            Dead(_attacker, true);
    }

    public async void Dead(Player _attacker, bool _dropReward)
    {
        _enemyAI.animator.SetBool(IsDead, true);
        Destroy(_enemyAI);
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Collider2D>().enabled    = false;

        if (_dropReward)
            DropReward(_attacker);

        //TODO: 적절한 방법이 아닌 것 같음
        if (OnEnemyHpChange?.GetInvocationList().Length > 0) //구독중이면
            BossHealthDisplay.Instance.UnSyncToBossHealthBar(this);

        //현재 재생중인 애니메이션이 종료될때 까지 대기
        await new WaitUntil(() => _enemyAI.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        await UniTask.Delay(TimeSpan.FromSeconds(1));
        PlaceCorpse();
        _objectPoolManager.ReturnObject(this);
    }

    private void DropReward(Player _attacker)
    {
        Vector3 _position = transform.position;

        EventBus.Publish(new ItemSpawnEvent(PickupType.Resource, GameManager.GetRandomCapsuleReward(PickupType.Resource) / 2, _position, _attacker),
                         new ItemSpawnEvent(PickupType.Exp,      GameManager.GetRandomCapsuleReward(PickupType.Exp)      / 2, _position, _attacker));
    }

    private void PlaceCorpse() { }

    private void GetDamage(int _damage)
    {
        stats.Health -= _damage;

        //TODO: BossHealthBarDisplay에 어떤식으로 연결시켜서 값을 동기화 시킬까
        OnEnemyHpChange?.Invoke(this, EventArgs.Empty);
    }

    private async void Knockback(Player _attacker, float _knockbackForce)
    {
        if (!stats.canKnockback)
            return;

        Vector2 _knockbackDirection = (transform.position - _attacker.PlayerPosition).normalized;

        float       _elapsedTime = 0;
        float       _duration    = 0.1f;
        MMTweenType _tween       = new(MMTween.MMTweenCurve.EaseInQuintic);

        while (_elapsedTime < _duration)
        {
            float _t = 1 - _tween.Evaluate(_elapsedTime / _duration);
            transform.Translate(_knockbackDirection * (_knockbackForce * _t));
            _elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    private void LevelUp(object _sender, EventArgs _args) { }

    public virtual void Attack()
    {
        List<Collider2D> _colliders = new();
        Physics2D.OverlapCollider(_attackCollider, new ContactFilter2D()
                                                   {
                                                       useLayerMask = true,
                                                       layerMask    = LayerMask.GetMask("Player")
                                                   }, _colliders);

        Debug.Log($"Captured colliders: {_colliders.Count}");
        foreach (Collider2D _player in _colliders)
        {
            if (_player.TryGetComponent(out Player _playerComponent)) //PickupRadius도 여기 걸려서 오류남 (지금은 레이어 분리하였음)
                _playerComponent.Attacked(_enemyAI.enemyBase.stats.AttackDamage, 0, _enemyAI.enemyBase);
        }
    }
}

public class EEnemyHpChange
{
    public event EventHandler OnEnemyHpChange;

    public void HpChanged()
    {
        OnEnemyHpChange?.Invoke(this, EventArgs.Empty);
    }
}
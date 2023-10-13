using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemyData> _spawnableEnemies;

    [Inject] private ObjectPoolManager _objectPoolManager;
    [Inject] private PlayerManager     _playerManager;

    [SerializeField] private const int MAX_ENEMIES = 30;
    [SerializeField] private       int currentEnemies;

    [SerializeField] private string taskStatus;
    [SerializeField] private bool   doSpawn;
    private const            int    INITIALIZE_DELAY = 5;
    private const            int    SPAWN_FREQUENCY  = 1;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayMapChangedEvent>(PlayMapChangedHandler);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayMapChangedEvent>(PlayMapChangedHandler);
    }

    private void Start()
    {
        DIContainer.Inject(this);
        _objectPoolManager.MakePool<EnemyBase>("Assets/_Prefabs/Entities/Enemy.prefab", 10);

        SpawnEnemy();
    }

    private void PlayMapChangedHandler(PlayMapChangedEvent _obj)
    {
        MapData _playMap = _obj.mapData.FirstOrDefault(_data => _data.currentMap is not BossPlayMap);

        if (_playMap is null)
        {
            Debug.Log("Not a play map");
            return;
        }

        _spawnableEnemies = _playMap.spawnableEnemies;
        Debug.Log("Spawnable enemies: " + _spawnableEnemies.Count);
    }

    private async UniTask SpawnEnemy()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(INITIALIZE_DELAY));

        while (true)
        {
            if (!doSpawn)
                return;

            if (currentEnemies >= MAX_ENEMIES)
                return;

            EnemyData _enemyData   = GetRandomEnemy();
            EnemyBase _enemyObject = _objectPoolManager.GetObject<EnemyBase>(false);

            _enemyObject.transform.position = MapManager.GetRandomPositionNearPlayer(_playerManager.GetRandomPlayer());

            _enemyObject.Initialize(_enemyData);
            _enemyObject.gameObject.SetActive(true);

            currentEnemies++;
            Debug.Log($"<color=green>Enemy spawned</color> : {_enemyData.name} in {_enemyObject.transform.position}");
            await UniTask.Delay(TimeSpan.FromSeconds(SPAWN_FREQUENCY));
        }
    }

    private EnemyData GetRandomEnemy()
    {
        return _spawnableEnemies[UnityEngine.Random.Range(0, _spawnableEnemies.Count)];
    }
}
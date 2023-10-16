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
    private const            int    SPAWN_FREQUENCY  = 25;

    private const int   DEFAULT_SPAWN_ENEMIES_ONCE_COUNT    = 3;
    private const float SPAWN_ENEMIES_DIFFICULTY_MULTIPLIER = 1.5f;

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

        UniTask _task = SpawnEnemyLoop();
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

    private async UniTask SpawnEnemyLoop()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(INITIALIZE_DELAY));

        while (true)
        {
            if (!doSpawn)
                return;

            if (currentEnemies >= MAX_ENEMIES)
                continue;

            int _spawnCount = Mathf.RoundToInt(DEFAULT_SPAWN_ENEMIES_ONCE_COUNT * DifficultyManager.NowDifficulty * SPAWN_ENEMIES_DIFFICULTY_MULTIPLIER);

            SpawnEnemies(_spawnCount);

            await UniTask.Delay(TimeSpan.FromSeconds(SPAWN_FREQUENCY));
        }
    }

    private async void SpawnEnemies(int _spawnCount)
    {
        await SpawnEnemy();

        for (int _i = 0; _i < _spawnCount - 1; _i++)
        {
            float _delay = UnityEngine.Random.Range(.3f, 1f);
            await SpawnEnemy(_delay);
        }
    }

    private async UniTask SpawnEnemy(float _delay = 0)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        
        EnemyData _enemyData   = GetRandomEnemy();
        EnemyBase _enemyObject = _objectPoolManager.GetObject<EnemyBase>(false);

        _enemyObject.transform.position = MapManager.GetRandomPositionNearPlayer(_playerManager.GetRandomPlayer());

        _enemyObject.Initialize(_enemyData);
        _enemyObject.gameObject.SetActive(true);

        currentEnemies++;
    }

    private EnemyData GetRandomEnemy()
    {
        return _spawnableEnemies[UnityEngine.Random.Range(0, _spawnableEnemies.Count)];
    }
}
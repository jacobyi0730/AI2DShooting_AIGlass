using UnityEngine;

public class JC_EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField, Min(0.1f)] private float minSpawnInterval = 1f;
    [SerializeField, Min(0.1f)] private float maxSpawnInterval = 3f;
    [SerializeField] private bool spawnOnStart = true;

    private bool _hasLoggedMissingSetup;
    private float _nextSpawnTime;

    private void Start()
    {
        ClampSpawnInterval();

        if (!HasValidSetup())
        {
            return;
        }

        if (spawnOnStart)
        {
            SpawnEnemy();
        }

        ScheduleNextSpawn(Time.time);
    }

    private void Update()
    {
        if (!HasValidSetup())
        {
            return;
        }

        if (Time.time < _nextSpawnTime)
        {
            return;
        }

        SpawnEnemy();
        ScheduleNextSpawn(Time.time);
    }

    private bool HasValidSetup()
    {
        if (enemyPrefab != null)
        {
            return true;
        }

        if (!_hasLoggedMissingSetup)
        {
            Debug.LogWarning($"[{nameof(JC_EnemySpawner)}] Enemy Prefab reference is missing on {name}.", this);
            _hasLoggedMissingSetup = true;
        }

        return false;
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = ResolveSpawnPoint();
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private Transform ResolveSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return transform;
        }

        int startIndex = Random.Range(0, spawnPoints.Length);
        for (int index = 0; index < spawnPoints.Length; index++)
        {
            int lookupIndex = (startIndex + index) % spawnPoints.Length;
            if (spawnPoints[lookupIndex] != null)
            {
                return spawnPoints[lookupIndex];
            }
        }

        return transform;
    }

    private void ScheduleNextSpawn(float currentTime)
    {
        _nextSpawnTime = currentTime + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void ClampSpawnInterval()
    {
        minSpawnInterval = Mathf.Max(0.1f, minSpawnInterval);
        maxSpawnInterval = Mathf.Max(minSpawnInterval, maxSpawnInterval);
    }
}

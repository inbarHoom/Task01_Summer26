using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave_Manager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waves = new();
    [SerializeField] private GameObject weakEnemy;
    [SerializeField] private GameObject strongEnemy;
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private GameObject player;

    private readonly List<Enemy_Combat> activeEnemies = new();
    private WaveData currentWave;
    private int currentWaveIndex;
    private int weakEnemiesStillToSpawn;
    private int strongEnemiesStillToSpawn;

    public static event System.Action<int> OnWaveUpdate;
    public static event System.Action OnFinishGame;

    public int CurrentWaveIndex => currentWaveIndex;
    public int AliveEnemyCount => activeEnemies.Count;

    private void Start()
    {
        if (waves.Count == 0)
        {
            Debug.LogError("Wave Manager has no waves assigned.");
            return;
        }

        if (SaveSession.HasPendingLoad)
        {
            RestoreFromSave(SaveSession.PendingLoadData);
        }
        else
        {
            StartWave(0);
        }

        StartCoroutine(RunCurrentWave());
    }

    private void StartWave(int waveIndex)
    {
        currentWaveIndex = waveIndex;
        currentWave = waves[currentWaveIndex];
        weakEnemiesStillToSpawn = currentWave.WeakCount;
        strongEnemiesStillToSpawn = currentWave.StrongCount;
        OnWaveUpdate?.Invoke(currentWaveIndex);
    }

    private IEnumerator RunCurrentWave()
    {
        while (weakEnemiesStillToSpawn > 0)
        {
            SpawnEnemy(weakEnemy, GetRandomSpawnPoint());
            weakEnemiesStillToSpawn--;
            yield return new WaitForSeconds(currentWave.Interval);
        }

        while (strongEnemiesStillToSpawn > 0)
        {
            SpawnEnemy(strongEnemy, GetRandomSpawnPoint());
            strongEnemiesStillToSpawn--;
            yield return new WaitForSeconds(currentWave.Interval);
        }

        while (activeEnemies.Count > 0)
        {
            yield return null;
        }

        int nextWaveIndex = currentWaveIndex + 1;
        if (nextWaveIndex >= waves.Count)
        {
            Time.timeScale = 0;
            OnFinishGame?.Invoke();
            yield break;
        }

        yield return new WaitForSeconds(currentWave.Interval + 1f);
        StartWave(nextWaveIndex);
        StartCoroutine(RunCurrentWave());
    }

    private void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
    {
        SpawnEnemy(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private Enemy_Combat SpawnEnemy(GameObject enemyPrefab, Vector3 position, Quaternion rotation)
    {
        GameObject spawnedObject = Pool_Manager.Instance.Spawn(enemyPrefab, position, rotation);
        spawnedObject.GetComponent<Enemy_Movement>().SetTarget(player);

        Enemy_Combat enemyCombat = spawnedObject.GetComponent<Enemy_Combat>();
        enemyCombat.Initialize(enemyPrefab);
        enemyCombat.OnDeath += HandleEnemyDeath;
        activeEnemies.Add(enemyCombat);
        return enemyCombat;
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    private void HandleEnemyDeath(Enemy_Combat enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void WriteSaveData(SaveGameData data)
    {
        data.waveIndex = currentWaveIndex;
        data.weakEnemiesStillToSpawn = weakEnemiesStillToSpawn;
        data.strongEnemiesStillToSpawn = strongEnemiesStillToSpawn;
        data.aliveEnemies.Clear();

        foreach (Enemy_Combat enemy in activeEnemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                continue;
            }

            data.aliveEnemies.Add(new EnemySaveData
            {
                enemyKind = enemy.SourcePrefab == strongEnemy ? EnemyKind.Strong : EnemyKind.Weak,
                position = enemy.transform.position,
                rotation = enemy.transform.rotation,
                health = enemy.CurrentHealth
            });
        }
    }

    private void RestoreFromSave(SaveGameData data)
    {
        currentWaveIndex = Mathf.Clamp(data.waveIndex, 0, waves.Count - 1);
        currentWave = waves[currentWaveIndex];
        weakEnemiesStillToSpawn = Mathf.Max(0, data.weakEnemiesStillToSpawn);
        strongEnemiesStillToSpawn = Mathf.Max(0, data.strongEnemiesStillToSpawn);
        OnWaveUpdate?.Invoke(currentWaveIndex);

        if (data.aliveEnemies == null)
        {
            return;
        }

        foreach (EnemySaveData savedEnemy in data.aliveEnemies)
        {
            GameObject enemyPrefab = savedEnemy.enemyKind == EnemyKind.Strong ? strongEnemy : weakEnemy;
            Enemy_Combat enemy = SpawnEnemy(enemyPrefab, savedEnemy.position, savedEnemy.rotation);
            enemy.RestoreHealth(savedEnemy.health);
        }
    }
}

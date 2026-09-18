using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Wave_Manager : MonoBehaviour
{
    [SerializeField] private List<WaveData> waves =  new List<WaveData>();
    private WaveData waveData;
    private int currentWaveIndex = 0;
    [SerializeField] private GameObject weakEnemy;
    [SerializeField] private GameObject strongEnemy;
    private int aliveEnemies = 0;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>() ;
    [SerializeField] private GameObject player;
    
    public static event Action<int> OnWaveUpdate;
    public static event Action OnFinishGame;
    

    void Start()
    {
        waveData = waves[currentWaveIndex];
        StartCoroutine(SpawnWave());
    }

    void HandleEnemyDeath()
    {
        aliveEnemies--;
    }
    
    

    private IEnumerator SpawnWave()
    {
        OnWaveUpdate.Invoke(currentWaveIndex);
        for (int i = 0; i < waveData.WeakCount; i++)
        {
            int pointIndex = Random.Range(0, spawnPoints.Count);
            SpawnEnemy(weakEnemy , spawnPoints[pointIndex]);
            yield return new WaitForSeconds(waveData.Interval);
        }
        for (int i = 0; i < waveData.StrongCount; i++)
        {
            int pointIndex = Random.Range(0, spawnPoints.Count);
            SpawnEnemy(strongEnemy , spawnPoints[pointIndex]);
            yield return new WaitForSeconds(waveData.Interval);
        }

        while (aliveEnemies>0)
        {
            yield return null;
        }
        Debug.Log("wave complete");
        if(currentWaveIndex + 1 < waves.Count)
        {
            currentWaveIndex++;
            yield return new WaitForSeconds(waveData.Interval+1f);
            waveData = waves[currentWaveIndex];
            StartCoroutine(SpawnWave());
        }
        else
        {
            Time.timeScale = 0;
            Debug.Log("All Waves Are Complete");
            OnFinishGame.Invoke();
        }
       
    }
    
    public void SpawnEnemy(GameObject enemyPrefab , Transform spawnPoint)
    {
      GameObject enemyObject = Pool_Manager.Instance.Spawn(enemyPrefab , spawnPoint.position, spawnPoint.rotation);
      enemyObject.GetComponent<Enemy_Movement>().SetTarget(player);
      Enemy_Combat enemy = enemyObject.GetComponent<Enemy_Combat>();
      enemy.Initialize(enemyPrefab);
      enemy.OnDeath -= HandleEnemyDeath;
      enemy.OnDeath += HandleEnemyDeath;
      aliveEnemies++;
      
    }
}

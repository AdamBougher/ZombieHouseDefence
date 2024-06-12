using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    public int maxEnemies;
    [FormerlySerializedAs("EnemyPrefabs")] public  List<GameObject> enemyPrefabs;
    [FormerlySerializedAs("SpawnPoints")] public Transform[] spawnPoints;

    private EnemyPool _enemyPool;


    public Vector2 spawnDelay = new(0.5f,2);


    private void Start()
    {
        _enemyPool = GetComponent<EnemyPool>();

        Enemy.HealthRange = new Vector2Int(1,2);

        StartCoroutine(StartWave());
        
        Enemy.OnLevelUp?.AddListener(OnLevelUp);
    }

    private IEnumerator StartWave()
    {
        
        while(true)
        {
            yield return new WaitUntil(() => GameManager.GamePaused == false);

            yield return new WaitForSeconds(Random.Range(spawnDelay.x,spawnDelay.y));

            yield return new WaitUntil(() => GameManager.GamePaused == false);
            yield return new WaitUntil(() => Enemy.EnemiesAlive < maxEnemies);
            
            //spawn new enemy
            SpawnEnemy();

        }
    }

    private Vector3 GetRandomSpawnPosition() 
    {
        Vector3 position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        position.z = 1;
        return position;
    }

    private void SpawnEnemy()
    {
        Enemy enemy = _enemyPool.GetPooledObject().GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.transform.position = GetRandomSpawnPosition();;
            enemy.gameObject.SetActive(true);
        }
    }

    private void OnLevelUp()
    {
        maxEnemies += 8;
    }
    
}

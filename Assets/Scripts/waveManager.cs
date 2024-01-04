using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int maxEnemys;
    public  List<GameObject> EnemyPrefabs;
    public Transform[] SpawnPoints;

    private EnemyPool enemyPool;


    public Vector2 spawnDelay = new(0.5f,2);


    void Start()
    {
        enemyPool = GetComponent<EnemyPool>();

        Enemy.healthRange = new Vector2Int(1,2);

        StartCoroutine(StartWave());
    }

    private IEnumerator StartWave()
    {
        
        while(true)
        {
            yield return new WaitUntil(() => GameManager.GamePaused == false);

            yield return new WaitForSeconds(Random.Range(spawnDelay.x,spawnDelay.y));

            yield return new WaitUntil(() => GameManager.GamePaused == false);
            yield return new WaitUntil(() => Enemy.EnemysAlive < maxEnemys);
            
            //spawn new enemy
            SpawnEnemy();

        }
    }

    private Vector3 GetRandomeSpawnPosition() 
    {
        Vector3 position = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position;
        position.z = 1;
        return position;
    }

    private void SpawnEnemy()
    {
        Enemy enemy = enemyPool.GetPooledObject().GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.transform.position = GetRandomeSpawnPosition();;
            enemy.gameObject.SetActive(true);
        }
    }
}

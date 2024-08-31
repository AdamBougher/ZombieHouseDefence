using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public int maxEnemies;
    public Transform[] spawnPoints;

    public ObjectPool<Enemy> enemyPool;
    public List<Enemy> bosses = new();
    public Vector2 spawnDelay = new(0.5f,2);


    private void Start() {
        enemyPool = EnemyPool.SharedInstance;
        StartCoroutine(StartWave());
        
        //GameTime.OnFifthMinuteTick += SpawnBoss;
        GameTime.OnFifthMinuteTick += SpawnBoss;
        GameTime.OnMinuetTick += OnLevelUp;
    }
    

    private IEnumerator StartWave() {
        while(true) {
            yield return new WaitUntil(() => GameManager.GamePaused == false);

            yield return new WaitForSeconds(Random.Range(spawnDelay.x,spawnDelay.y));

            yield return new WaitUntil(() => GameManager.GamePaused == false);
            yield return new WaitUntil(() => Enemy.EnemiesAlive < maxEnemies);
            
            //spawn new enemy
            SpawnEnemy();

        }
        // ReSharper disable once IteratorNeverReturns
    }

    private Vector3 GetRandomSpawnPosition() {
        Vector3 position;
        do {
            position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        } while (Physics2D.OverlapPoint(position, LayerMask.GetMask("nospawn")) is not null);

        position.z = 1;
        return position;
    }

    private void SpawnEnemy()
    {
        var enemy = enemyPool.GetPooledObject();

        if (enemy is null) 
            return;
        
        enemy.transform.position = GetRandomSpawnPosition();
        enemy.gameObject.SetActive(true);
    }

    private void SpawnBoss(object sender, EventArgs e) {
        var bossPrefab = bosses[Random.Range(0, bosses.Count)];
        var boss = Instantiate(bossPrefab, GetRandomSpawnPosition(), Quaternion.identity, transform.root.root);
        boss.gameObject.SetActive(true);
    }
    
    private void OnLevelUp(object sender, EventArgs e)
    {
        maxEnemies += 8;
    }
    
}

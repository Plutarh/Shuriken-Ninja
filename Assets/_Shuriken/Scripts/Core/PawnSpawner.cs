using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PawnSpawner : MonoBehaviour
{
    public AIEnemy pawnPrefab;

    public float spawnCount;
    public float spawnDelay;

    float spawnTimer;
    
    IEnemyFactory _enemyFactory;

    public event Action<AIEnemy> OnPawnSpawn;

    [Inject]
    void Construct(IEnemyFactory enemyFactory)
    {
        _enemyFactory = enemyFactory;

        if(_enemyFactory != null)
        {
            _enemyFactory.Load();
        }
    }

    void Start()
    {
       
    }

    void Update()
    {

    }

    public void SpawnPawn()
    {
        if (spawnCount <= 0) return;
        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnDelay)
        {
            AIEnemy createdEnemy = _enemyFactory.Create(pawnPrefab,transform.position) as AIEnemy;
            spawnTimer = 0;
            spawnCount--;
            OnPawnSpawn?.Invoke(createdEnemy);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}

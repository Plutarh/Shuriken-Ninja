using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PawnSpawner : MonoBehaviour
{
    public Pawn pawnPrefab;

    public float spawnCount;
    public float spawnDelay;

    float spawnTimer;
    
    IEnemyFactory _enemyFactory;

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
        SpawnPawn();
    }

    void SpawnPawn()
    {
        if (spawnCount <= 0) return;
        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnDelay)
        {
            //Instantiate (pawnPrefab, transform.position, Quaternion.identity);
            _enemyFactory.Create(pawnPrefab,transform.position);
            spawnTimer = 0;
            spawnCount--;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}

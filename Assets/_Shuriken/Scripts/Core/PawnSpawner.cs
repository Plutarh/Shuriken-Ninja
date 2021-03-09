using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PawnSpawner : MonoBehaviour
{
    public List<AIEnemy> pawnPrefabs = new List<AIEnemy>();
    
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

    public AIEnemy SpawnPawn(int index ,Vector3 spawPos)
    {
        var foundedEnemyVariant = pawnPrefabs.FirstOrDefault(pp => pp.enemyTypeIndex == index);
        AIEnemy createdEnemy = _enemyFactory.Create(foundedEnemyVariant, spawPos) as AIEnemy;
        OnPawnSpawn?.Invoke(createdEnemy);
        return createdEnemy;
    }

    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);*/
    }
}

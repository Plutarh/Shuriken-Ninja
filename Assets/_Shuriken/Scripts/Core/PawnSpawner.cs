﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PawnSpawner : MonoBehaviour
{
    public AIEnemy pawnPrefab;
    
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

    public void SpawnPawn(Transform spawPos)
    {
        AIEnemy createdEnemy = _enemyFactory.Create(pawnPrefab, spawPos.position) as AIEnemy;
        OnPawnSpawn?.Invoke(createdEnemy);
    }

    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);*/
    }
}

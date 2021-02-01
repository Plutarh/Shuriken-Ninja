using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyFactory : MonoBehaviour, IEnemyFactory
{
    [SerializeField] Pawn enemyPawnPrefab;

    DiContainer _diContainer;

   
    /*
    public EnemyFactory(DiContainer diContainer)
    {
      
        _diContainer = diContainer;
    }*/

    

    public void Load()
    {

    }

    

    public void Create(Vector3 spawnPos)
    {
        _diContainer.InstantiatePrefab(enemyPawnPrefab, spawnPos, Quaternion.identity,null);
    }
}

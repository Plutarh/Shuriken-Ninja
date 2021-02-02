using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyFactory :  IEnemyFactory
{
    Object enemyPawnPrefab;

    DiContainer _diContainer;
    
    public EnemyFactory(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    // Возможно,когда то сделаю загрузку из ресурсов,если очень надо будет
    public void Load()
    {
        enemyPawnPrefab = Resources.Load("Base Ninja Enemy");
    }

    public void Create(Object enemy,Vector3 spawnPos)
    {
        _diContainer.InstantiatePrefab(enemy, spawnPos, Quaternion.identity,null);
    }
}

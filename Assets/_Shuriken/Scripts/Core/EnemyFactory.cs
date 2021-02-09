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

    public AIEnemy Create(AIEnemy enemy,Vector3 spawnPos)
    {
        return _diContainer.InstantiatePrefabForComponent<AIEnemy>(enemy, spawnPos, Quaternion.identity,null);
    }
}

using UnityEngine;

public interface IEnemyFactory
{
    void Create(Object enemyToCreate,Vector3 spawnPos);
    void Load();
}
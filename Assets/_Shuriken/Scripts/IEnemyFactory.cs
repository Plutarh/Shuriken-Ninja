using UnityEngine;

public interface IEnemyFactory
{
    AIEnemy Create(AIEnemy enemyToCreate,Vector3 spawnPos);
    void Load();
}
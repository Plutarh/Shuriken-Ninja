using UnityEngine;

public interface IEnemyFactory
{
    void Create(Vector3 spawnPos);
    void Load();
}
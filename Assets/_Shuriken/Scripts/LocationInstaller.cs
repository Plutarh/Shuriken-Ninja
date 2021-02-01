using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    public Transform startPoint;
    public GameObject playerPrefab;


    public List<Transform> enemySpawnPoints = new List<Transform>();

    public override void InstallBindings()
    {
        BindPlayer();
        //SpawnEnemy();
    }

    private void BindPlayer()
    {
        PlayerController playerController = Container
            .InstantiatePrefabForComponent<PlayerController>(playerPrefab, startPoint.position, Quaternion.identity, null);

        Container
            .Bind<PlayerController>()
            .FromInstance(playerController)
            .AsSingle();

       
    }

    private void SpawnEnemy()
    {
        var enemyFactory = Container.Resolve<IEnemyFactory>();

        // Может буду грузить из ресурсов,пока полежит
        enemyFactory.Load();
        foreach (var sp in enemySpawnPoints)
        {
            enemyFactory.Create(sp.position);
        }

    }
}

using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    public Transform startPoint;
    public GameObject playerPrefab;

    public PlayerController playerInstance;
    public List<Transform> enemySpawnPoints = new List<Transform>();

    public override void InstallBindings()
    {
        BindPlayer();
        BindEnemyFactory();

        //Initialize();
    }

    private void BindPlayer()
    {
        PlayerController playerController = Container
            .InstantiatePrefabForComponent<PlayerController>(playerPrefab, startPoint.position, Quaternion.identity, null);

        Container
            .Bind<PlayerController>()
            .FromInstance(playerController)
            .AsSingle();

        playerInstance = playerController;
    }

    private void BindEnemyFactory()
    {
        Container
            .Bind<IEnemyFactory>()
            .To<EnemyFactory>()
            .AsSingle();
    }

    /*
    public void Initialize()
    {
        var enemyFactory = Container.Resolve<IEnemyFactory>();
        enemyFactory.Load();

        foreach (var esp in enemySpawnPoints)
        {
            enemyFactory.Create(esp.position);
        }
    }*/
}

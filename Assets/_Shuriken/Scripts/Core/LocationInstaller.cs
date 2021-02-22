using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    public Transform startPoint;
    public GameObject playerPrefab;

    public PlayerController playerInstance;

    public Transform runPoint;
    public LevelSessionService levelSession;

    private void OnValidate()
    {
        if (this != null && startPoint == null)
        {
            var fo = GameObject.Find("StartPoint");
            if (fo) startPoint = fo.transform;
        }
    }
    public override void InstallBindings()
    {
        BindLevelSessionService();
        BindPlayer();
        BindEnemyFactory();
        
    }

    private void BindPlayer()
    {
        PlayerController playerController = Container
            .InstantiatePrefabForComponent<PlayerController>(playerPrefab, startPoint.position, Quaternion.identity, null);

        Container
            .Bind<PlayerController>()
            .FromInstance(playerController)
            .AsSingle()
            .NonLazy();

        playerInstance = playerController;
    }

    private void BindEnemyFactory()
    {
        Container
            .Bind<IEnemyFactory>()
            .To<EnemyFactory>()
            .AsSingle()
            .NonLazy();
    }

    private void BindLevelSessionService()
    {
        Container
            .Bind<LevelSessionService>()
            .FromInstance(levelSession)
            .AsSingle()
            .NonLazy();
    }
}

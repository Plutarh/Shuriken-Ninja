using UnityEngine;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    public Transform startPoint;
    public GameObject playerPrefab;
    public override void InstallBindings()
    {
        PlayerController playerController = Container
            .InstantiatePrefabForComponent<PlayerController>(playerPrefab,startPoint.position,Quaternion.identity,null);

        Container
            .Bind<PlayerController>()
            .FromInstance(playerController)
            .AsSingle();
    }
}

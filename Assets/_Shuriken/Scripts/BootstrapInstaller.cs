﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{

    public EventService eventServiceInstance;
    public Core coreInstance;
    public InputService inputServiceInstace;

    public override void InstallBindings()
    {
        BindEventService();
        BindCoreInstance();
        BindInputService();
    }

    private void BindEventService()
    {
        Container
            .Bind<EventService>()
            .FromInstance(eventServiceInstance)
            .AsSingle();
    }

    private void BindCoreInstance()
    {
        Container
           .Bind<Core>()
           .FromInstance(coreInstance)
           .AsSingle();
    }

    private void BindInputService()
    {
        Container
          .Bind<IInputService>()
          .To<InputService>()
          .FromInstance(inputServiceInstace)
          .AsSingle();
    }
}

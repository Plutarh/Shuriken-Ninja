﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{

    public EventService eventServiceInstance;
    public Core coreInstance;
    public InputService inputServiceInstace;
    public TimeControllService timeControllInstance;

    public override void InstallBindings()
    {
        BindEventService();
        BindCoreInstance();
        BindInputService();
        BindTimeControllService();
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
           .AsSingle()
           .NonLazy();
       
    }

    private void BindTimeControllService()
    {
        Container
            .Bind<TimeControllService>()
            .FromInstance(timeControllInstance)
            .AsSingle()
            .NonLazy();
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

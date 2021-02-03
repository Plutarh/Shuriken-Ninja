using BzKovSoft.CharacterSlicerSamples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AIEnemy : Pawn
{

    public PlayerController player;

    public event Action<AIEnemy> OnDeath;

    [Inject]
    void Construct(PlayerController playerInstance)
    {
        player = playerInstance;
    }

    private void OnValidate()
    {
        base.Init();
    }
    private void Awake()
    {
        characterSlicer.OnSlicedFinish += OnSlice;
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!characterSlicer.sliced)
        {

            navMeshAgent.SetDestination(player.transform.position);
        }
        else
        {
            if(navMeshAgent != null)
            {
                if (!navMeshAgent.isStopped)
                {
                    OnSlice();
                }
            }
        }
    }

    void OnSlice()
    {
        if (navMeshAgent == null) return;
        navMeshAgent.isStopped = true;
        OnDeath?.Invoke(this);
        ClearComponents();
        Destroy(gameObject, 2f);
    }

    void ClearComponents()
    {
        Destroy(navMeshAgent);
        navMeshAgent = null;
        body = null;
        animator = null;
        adderSliceable = null;
    }

    private void OnDestroy()
    {
        characterSlicer.OnSlicedFinish -= OnSlice;
    }
}

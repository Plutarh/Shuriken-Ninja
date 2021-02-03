using BzKovSoft.CharacterSlicerSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AIEnemy : Pawn
{

    public PlayerController player;


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
        navMeshAgent.SetDestination(player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!characterSlicer.sliced)
        {
           
           
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

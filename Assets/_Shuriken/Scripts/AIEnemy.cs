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
            if (!navMeshAgent.isStopped) navMeshAgent.isStopped = true;
        }
    }

    void OnSlice()
    {
        //Debug.Log(gameObject.name + " dead");
        navMeshAgent.isStopped = true;
    }

    private void OnDestroy()
    {
        characterSlicer.OnSlicedFinish -= OnSlice;
    }
}

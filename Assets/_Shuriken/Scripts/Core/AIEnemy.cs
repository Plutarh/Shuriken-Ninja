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

    public Transform rootObj;
    public ParticleSystem deathParticle;

    public float distanceToTarget;

    public EAIState aiState;
    public enum EAIState
    {
        Chaise,
        Attack,
        Idle,
        Win
    }

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
        adderSliceable.SetupSliceableParts(this);
        characterSlicer.OnSlicedFinish += OnSlice;

        ChangeState(EAIState.Chaise);
    }

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        AIStateMachine();
    }

    public void ChangeState(EAIState newState)
    {
        if (aiState == newState) return;
        aiState = newState;

        switch (aiState)
        {
            case EAIState.Chaise:
                break;
            case EAIState.Attack:
                navMeshAgent.isStopped = true;
                animator.CrossFade("Melee Attack", 0.2f);
                break;
            case EAIState.Idle:
                animator.Play("Idle");
                break;
            case EAIState.Win:
                break;
        }

    }

    void AIStateMachine()
    {
        switch (aiState)
        {
            case EAIState.Chaise:

                Chase();

                break;
            case EAIState.Attack:
                RotateToTarget();
                break;
            case EAIState.Idle:
                animator.CrossFade("Idle", 0.2f);
                break;
            case EAIState.Win:
                break;
        }
    }

    void RotateToTarget()
    {
        if (player == null) return;

        Vector3 targetPos = player.transform.position - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation
              , Quaternion.LookRotation(targetPos, Vector3.up)
              , Time.deltaTime * 1);
    }

    void Chase()
    {
        if(player != null)
        {
            distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToTarget < 1.5f)
            {
                if (!navMeshAgent.isStopped) navMeshAgent.isStopped = true;
                ChangeState(EAIState.Attack);
            }
        }

        if (!characterSlicer.sliced)
        {
            navMeshAgent.SetDestination(player.transform.position);
        }
        else
        {
            if (navMeshAgent != null)
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
        if (dead) return;
        if (navMeshAgent == null) return;
        navMeshAgent.isStopped = true;
        OnDeath?.Invoke(this);
        ClearComponents();
        dead = true;
        StartCoroutine(IEWaitToDestroy());
    }

    IEnumerator IEWaitToDestroy()
    {
        yield return new WaitForSecondsRealtime(2f);
        var puff = Instantiate(deathParticle, rootObj.position,Quaternion.identity);
        puff.transform.SetParent(null);
        //Destroy(puff.gameObject, 3f);
        Destroy(gameObject);
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

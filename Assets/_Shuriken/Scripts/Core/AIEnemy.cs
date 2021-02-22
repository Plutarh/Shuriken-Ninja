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

    public Weapon weapon;
   
    public enum EAIState
    {
        Chaise,
        Attack,
        Idle,
        Win,
        Death
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
        if (characterSlicer.sliceSide == CharacterSlicerSampleFast.ESliceSide.NotSliced)
        {
            adderSliceable.SetupSliceableParts(this);
            characterSlicer.OnSlicedFinish += Death;
            ChangeState(EAIState.Chaise);
            EventService.OnPlayerDead += () => ChangeState(EAIState.Win);
        }
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
        if (dead) return;
        if (aiState == newState) return;
        aiState = newState;

        switch (aiState)
        {
            case EAIState.Chaise:
                break;
            case EAIState.Attack:
                if (navMeshAgent != null) navMeshAgent.isStopped = true;
                if (animator != null) animator.CrossFade("Melee Attack", 0.2f);

                break;
            case EAIState.Idle:
               
                break;
            case EAIState.Win:
                if (navMeshAgent != null) navMeshAgent.isStopped = true;
                animator.CrossFade("Idle", 0.2f);
                break;
            case EAIState.Death:
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
                //animator.CrossFade("Idle", 0.2f);
                break;
            case EAIState.Win:
                break;
            case EAIState.Death:
                break;
        }

        if(characterSlicer.sliced)
        {
            if (navMeshAgent != null)
            {
                if (!navMeshAgent.isStopped)
                {
                    Death();
                }
            }
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
        if (dead) return;
        if(player != null)
        {
            distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToTarget < 1.5f)
            {
                ChangeState(EAIState.Attack);
            }
        }

        if (!characterSlicer.sliced)
        {
            navMeshAgent.SetDestination(player.transform.position);
        }
     
    }

    public override void Death()
    {
        if (dead) return;
        if (navMeshAgent == null) return;
        
        if(weapon != null)
        {
            weapon.gameObject.AddComponent<Rigidbody>();
            weapon.transform.SetParent(null);
            Destroy(weapon.gameObject, 3f);
        }
        navMeshAgent.avoidancePriority = 50;
        navMeshAgent.isStopped = true;
        OnDeath?.Invoke(this);
        ClearComponents();
        dead = true;
        //ChangeState(EAIState.Death);
        StartCoroutine(IEWaitToDestroy());
    }

    public override void TakeDamage(float dmg)
    {
        if (!dead)
            StartCoroutine(IEHitCoro(1.5f));
        health.heathPoint -= dmg;
        if(health.heathPoint <= 0)
        {
            // =(
        }
    }

    IEnumerator IEHitCoro(float duration)
    {
        
        EAIState oldState = aiState;
        ChangeState(EAIState.Idle);
        animator.CrossFade("Hit", 0.1f);

        if (navMeshAgent != null) navMeshAgent.isStopped = true;
     
        var dur = duration;
        while(dur > 0)
        {
            dur -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ChangeState(oldState);
        if(navMeshAgent != null) navMeshAgent.isStopped = false;
       
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
        characterSlicer.OnSlicedFinish -= Death;
        EventService.OnPlayerDead -= () => ChangeState(EAIState.Win);
    }
}

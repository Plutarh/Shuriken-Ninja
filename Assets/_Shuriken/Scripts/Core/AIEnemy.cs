using BzKovSoft.CharacterSlicerSamples;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class AIEnemy : Pawn
{

    public PlayerController player;

    public event Action<AIEnemy> OnDeath;

    public Transform rootObj;
  

    public float distanceToTarget;

    public EAIState aiState;

    public Weapon weapon;

    [Header("FX")]
    public ParticleSystem deathParticle;
    public ParticleSystem disappearParticle;

    public bool dummy;
    public int enemyTypeIndex;
    public bool spawnedByPoint;

    public enum EAIState
    {
        Null,
        Chaise,
        Attack,
        Idle,
        Win,
        Death
    }

    List<Coroutine> allActivesCoroutines = new List<Coroutine>();

    public SkinnedMeshRenderer skinnedMesh;
    public Material myMat;

    [Inject]
    void Construct(PlayerController playerInstance)
    {
        if (dummy) return;
        player = playerInstance;
    }

    private void OnValidate()
    {
        base.Init();
    }
    public override void Awake()
    {
        base.Awake();

        if (dummy)
        {
            Destroy(navMeshAgent);
            return;
        }
        if (characterSlicer.sliceSide == CharacterSlicerSampleFast.ESliceSide.NotSliced)
        {
            if(adderSliceable != null)
                adderSliceable.SetupSliceableParts(this);
            characterSlicer.OnSlicedFinish += Death;
          
            EventService.OnPlayerDead += () => ChangeState(EAIState.Win);
        }
        else
        {
            Debug.LogError("Sliced comp");
            Destroy(gameObject, 2f);
        }

        //skinnedMesh = GetComponent<SkinnedMeshRenderer>();
        myMat = skinnedMesh.material;

        weapon = GetComponentsInChildren<Weapon>().ToList().FirstOrDefault(w => w.gameObject.activeSelf);
        if(weapon)
            weapon.SetOwner(this);
    }

    void Start()
    {
        if (spawnedByPoint) ChangeState(EAIState.Chaise);
        else ChangeState(EAIState.Idle);
        if (dummy) ChangeState(EAIState.Win);
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
                if (animator != null) animator.CrossFade("Chaise",0.2f);
                break;
            case EAIState.Attack:
                if (navMeshAgent != null) navMeshAgent.isStopped = true;
                if (animator != null) animator.CrossFade("Melee Attack", 0.2f);

                break;
            case EAIState.Idle:
                if (animator != null) animator.CrossFade("Idle", 0.1f);
                break;
            case EAIState.Win:
                if (navMeshAgent != null) navMeshAgent.isStopped = true;
                if(animator != null) animator.CrossFade("Win", 0.2f);
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
              /*
                if (!navMeshAgent.isStopped)
                {
                  
                }*/
                Death();
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
        if (dummy) return;
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
            if(navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(player.transform.position);
            }
              
            else
            {
                Debug.LogError("Nav mesh Fuck");
            }
        }
     
    }

    public override void Death()
    {
        if (dead) return;
        if (navMeshAgent == null) return;
        
        if(weapon != null)
        {
            var weaponRB = weapon.gameObject.GetComponent<Rigidbody>();
            if(weaponRB != null)
            {
                weaponRB.useGravity = true;
                weaponRB.isKinematic = false;
            }
         
            weapon.transform.SetParent(null);
            Destroy(weapon.gameObject, 3f);
        }
        navMeshAgent.avoidancePriority = 50;
        navMeshAgent.isStopped = true;
        OnDeath?.Invoke(this);
        dead = true;
        ClearComponents();
        StartCoroutine(IEWaitToDestroy());
    }

    #region STATUS_EFFECT

    Coroutine freezeStatus;
    public override void TakeStatus(EStatusEffect statusEffect)
    {
       
        if (dead) return;

        switch (statusEffect)
        {
            case EStatusEffect.Freeze:

                if (freezeStatus == null)
                {
                    freezeStatus = StartCoroutine(IEFreezeStatus());
                    allActivesCoroutines.Add(freezeStatus);
                }
                else
                {
                    fullFreezeTime = 2;
                }


                break;
            case EStatusEffect.Poison:
                break;
        }
    }

    public Color freezeColor;
    float fullFreezeTime = 0;
    IEnumerator IEFreezeStatus()
    {
        float navMeshSpeed = navMeshAgent.speed;
        float animatorSpeed = animator.speed;
        fullFreezeTime = 2;

        navMeshAgent.speed = 0;
        animator.speed = 0;

        myMat.DOColor(freezeColor, "_BaseColor", 0.3f);
      
        //myMat.EnableKeyword("_EMISSION");
        //myMat.SetColor("_EmissionColor", Color.blue * 1f);

        while (fullFreezeTime > 0)
        {
            fullFreezeTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //myMat.DisableKeyword("_EMISSION");
        myMat.DOColor(Color.white,"_BaseColor",1);
        DOTween.To(()=> navMeshAgent.speed, x=> navMeshAgent.speed = x, navMeshSpeed,1);
        DOTween.To(() => animator.speed, x => animator.speed = x, animatorSpeed, 1);
        //animator.speed = animatorSpeed;

        freezeStatus = null;
        yield return null;
    }


    #endregion

    #region DAMAGE

    Coroutine hitImpact;

    public override void TakeDamage(float dmg,Vector3 dir, EDamageType damageType)
    {
       
        health.heathPoint -= dmg;
        if(health.heathPoint <= 0)
        {
          
            if (dir.y < 0) dir.y = 1;
            if(navMeshAgent != null)
            {
                navMeshAgent.isStopped = true;
                
            }
            switch (damageType)
            {
                case EDamageType.Hit:
                    
                    characterSlicer.ConvertToRagdollSimple(dir * 2 + Vector3.up, Vector3.zero);
                    break;
                case EDamageType.Explosion:
                    characterSlicer.ConvertToRagdollSimple(Vector3.up  * 2 + dir.normalized * 5, Vector3.zero);
                    break;
            }

            //Debug.LogError(dir.normalized + " MOVE DIR");
            Death();
        }
        else
        {
            if (!dead && hitImpact == null)
            {
                hitImpact = StartCoroutine(IEHitCoro(1f));
                allActivesCoroutines.Add(hitImpact);
            }
               
        }
    }

    IEnumerator IEHitCoro(float duration)
    {
        
        EAIState oldState = aiState;
        ChangeState(EAIState.Idle);
        if(animator != null)
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
        hitImpact = null;
    }

    #endregion

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

        foreach (var activeCoro in allActivesCoroutines)
        {
            if (activeCoro != null) StopCoroutine(activeCoro);
        }
        allActivesCoroutines.Clear();

        navMeshAgent = null;
        body = null;
        animator = null;
        adderSliceable = null;
       
    }

    public void Disappear()
    {
        var particle = Instantiate(disappearParticle, transform.position + Vector3.up, Quaternion.identity);
        particle.transform.SetParent(null);
        Destroy(gameObject);
        Destroy(particle.gameObject, 3f);
    }

    private void OnDestroy()
    {
        characterSlicer.OnSlicedFinish -= Death;
        EventService.OnPlayerDead -= () => ChangeState(EAIState.Win);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerController : Pawn
{
    public Vector3 shurikenOffset;
    //public SimpleMover shurikenPrefab;

    public Transform R_shurikenSpawnPos;
    public Transform L_shurikenSpawnPos;
    public Vector3 throwDirection;
    public Vector3 throwPoint;
    public Vector3 relPoint;

    public Transform runPoint;

    public EPlayerState playerState;

    public AIEnemy closestEnemy;
    public float closestDist;


    public Weapon baseWeaponPrefab;
    public Weapon greatWeaponPrefab;
    IThrowable throwableObject;

    public float heightMultiply;

    public int powerThrow;

    [Header("FX")]
    [SerializeField] ParticleSystem wakeUpParticle;

    public enum EPlayerState
    {
        MoveToPoint,
        Stand,
        Death,
        WakeUp
    }

    bool shotLeft;
    bool shotMega;
    bool blockShot;
    GameObject throwTarget;
    Collider targetCol;

    IInputService _inputService;
    LevelSessionService levelSession;
    [Inject]
    void Construct(IInputService inputService , WeaponManager weaponManager)
    {
        _inputService = inputService;
        _inputService.OnColliderClick += GetThrowDirection;
        _inputService.OnNonColliderClick += GetThrowDirection;

        weaponManager.playerInstance = this;
    }

    public void SetLevelSessionService(LevelSessionService instance)
    {
        levelSession = instance;
    }

    private void OnValidate()
    {
        base.Init();
    }

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        EventService.OnEnemyHit += OnEnemyHit;
        EventService.OnHitEnemyHead += OnHitEnemyHead;
        EventService.OnGameOver += OnGameOver;
    }

    

  

    void Start()
    {
        UIController.Get.SetPlayerThrowPoint(powerThrow);
    }

   
    // Update is called once per frame
    void Update()
    {
        StateMachine();
        CheckDistanceToCurrentRunPoint();

    }

    private void FixedUpdate()
    {
        
    }

    void OnGameOver(EventService.EGameState gameState)
    {
        switch (gameState)
        {
            case EventService.EGameState.Win:
                break;
            case EventService.EGameState.Loose:
                break;
            case EventService.EGameState.Continue:
                ChangeState(EPlayerState.WakeUp);
                break;
        }
    }

    private void OnEnemyHit()
    {
        if (powerThrow < 5)
        {
            powerThrow++;
            if (powerThrow >= 5)
            {
                powerThrow = 5;
                shotMega = true;
            }
        }
        else
        {
            powerThrow = 5;
            shotMega = true;
        }

        UIController.Get.SetPlayerThrowPoint(powerThrow);
    }

    private void OnHitEnemyHead()
    {
        if (powerThrow < 5)
        {
            powerThrow += 3;
            if (powerThrow >= 5)
            {
                powerThrow = 5;
                shotMega = true;
            }
        }
        else
        {
            powerThrow = 5;
            shotMega = true;
        }
        UIController.Get.SetPlayerThrowPoint(powerThrow);
    }

    void CheckDistanceToCurrentRunPoint()
    {
        if (dead) return;
        if (runPoint == null) return;
        if (Vector3.Distance(transform.position, runPoint.position) < 0.5f)
        {
            ChangeState(EPlayerState.Stand);
           
        }
    }
 
    void StateMachine()
    {
        if (levelSession.levelState == LevelSessionService.ELevelState.NotStarted) return;
        // Every frame call
        switch (playerState)
        {
            case EPlayerState.MoveToPoint:
                break;
            case EPlayerState.Stand:

                FindClosestEnemy();
                RotateToClosestTarget();
                break;
            case EPlayerState.Death:
                break;
            case EPlayerState.WakeUp:
                break;
        }


    }

    public void ChangeState(EPlayerState newState)
    {
        if (playerState == newState) return;
        playerState = newState;

        // Single call
        switch (playerState)
        {
            case EPlayerState.MoveToPoint:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = 4;

                break;
            case EPlayerState.Stand:
                EventService.OnPlayerRanActionPoint?.Invoke();
                ResetAttackTriggers();
                navMeshAgent.isStopped = true;
                blockShot = false;
                animator.SetBool("Run", false);
                animator.CrossFade("Idle", 0.1f);
                break;
            case EPlayerState.Death:
                navMeshAgent.isStopped = true;
                animator.CrossFade("Death", 0.1f);
                EventService.OnPlayerDead?.Invoke();
                dead = true;
                ResetAttackTriggers();
                break;
            case EPlayerState.WakeUp:

                StartCoroutine(IEWakeUp());
                break;
        }
    }

    IEnumerator IEWakeUp()
    {
        ResetAttackTriggers();
        
        animator.CrossFade("Wake Up", 0.2f);
        float delay = 1.6f;
        while(delay > 0)
        {
            delay -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
       

    }

    public void WakeUpEffect()
    {
        if (!wakeUpParticle.isPlaying) wakeUpParticle.Play();
        EventService.OnPlayerWakedUp?.Invoke();
        dead = false;
        health.heathPoint = 1;
    }

    void ResetAttackTriggers()
    {
        animator.ResetTrigger("ThrowR");
        animator.ResetTrigger("ThrowL");
        animator.ResetTrigger("ThrowR");
    }

    public void MoveToPoint(Transform point)
    {
        //return;
        runPoint = point;
        if(navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) 
            navMeshAgent.SetDestination(runPoint.position);

        animator.SetBool("Run", true);
        animator.CrossFade("Run", 0.1f);
        ChangeState(EPlayerState.MoveToPoint);
    }


  
    void GetThrowDirection(Vector3 point)
    {
        throwTarget = null;
        throwPoint = transform.position + (point * 10) + (Vector3.up * heightMultiply);

        /*
        Debug.LogError("MEWO " + Vector3.Distance(transform.position, throwPoint));

        if (Vector3.Distance(transform.position,throwPoint) < 9)
        {
            Debug.DrawLine(R_shurikenSpawnPos.position, throwPoint + Vector3.up, Color.red, 5f);
            throwPoint += Vector3.up;
          
        }*/
     

        Debug.DrawLine(R_shurikenSpawnPos.position, throwPoint, Color.white, 3f);
        Debug.DrawLine(L_shurikenSpawnPos.position, throwPoint, Color.white, 3f);

        ThrowAnimation(false);
    }

    
    void GetThrowDirection(Vector3 point,Collider goCollider)
    {
        //if (playerState != EPlayerState.Stand) return;
        targetCol = goCollider;

        var pawnCol = targetCol.transform.root.GetComponent<Pawn>();

        if(pawnCol != null && pawnCol.dead) targetCol = null;
       
       

        throwPoint = point;

        relPoint = R_shurikenSpawnPos.InverseTransformPoint(point);

        Debug.DrawLine(R_shurikenSpawnPos.position, point, Color.blue, 3f);

      
        ThrowAnimation();
    }

    void ThrowAnimation(bool nullTarget = false)
    {
        if (levelSession.levelState == LevelSessionService.ELevelState.NotStarted) return;

        if (shotMega)
        {
            if (!blockShot)
            {
                animator.SetTrigger("ThrowM");
                blockShot = true;
               
                powerThrow = 0;
                UIController.Get.SetPlayerThrowPoint(powerThrow);
            }
        }
        else
        {
            if (shotLeft)
            {
                if (!blockShot)
                {
                    animator.SetTrigger("ThrowR");
                    blockShot = true;
                    if(nullTarget)
                        targetCol = null;
                }


            }
            else
            {
                if (!blockShot)
                {
                    animator.SetTrigger("ThrowL");
                    blockShot = true;
                    if(nullTarget)
                        targetCol = null;
                }

            }
        }
    }

    public void ThrowShurikenByAnimator()
    {
        
        CreateThrowableWeapon(relPoint);
        //Debug.DrawLine(R_shurikenSpawnPos.transform.position, relPoint, Color.cyan, 2);
    }

    void CreateThrowableWeapon(Vector3 relPoint)
    {
        if (shotMega)
        {
            throwableObject = Instantiate(greatWeaponPrefab, R_shurikenSpawnPos.position + shurikenOffset, Quaternion.identity) as IThrowable;
          
            shotMega = false;
        }
        else
        {
            if (shotLeft)
            {
                throwableObject = Instantiate(baseWeaponPrefab, R_shurikenSpawnPos.position, Quaternion.identity) as IThrowable;
            }
            else
            {

                throwableObject = Instantiate(baseWeaponPrefab, L_shurikenSpawnPos.position, Quaternion.identity) as IThrowable;
            }
            shotLeft = !shotLeft;
        }
     
        blockShot = false;

        if (throwableObject as Weapon)
        {
            (throwableObject as Weapon).SetOwner(this);
        }



        if (throwableObject.IsSlicer())
        {
            throwableObject.SetMoveType(Shuriken.EMoveType.Free);
        }
        else
        {
            if (targetCol != null)
            {
                throwableObject.SetMoveType(Shuriken.EMoveType.Target);
                throwableObject.SetTargetCollider(targetCol);
            }
            else
            {
                throwableObject.SetMoveType(Shuriken.EMoveType.Free);
            }
        }

      


        throwableObject.SetTargetPosition(throwPoint);
        targetCol = null;
    }

  
    void RotateToClosestTarget()
    {
        if (closestEnemy == null) return;

        Vector3 targetPos = closestEnemy.transform.position - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation
              , Quaternion.LookRotation(targetPos, Vector3.up)
              , Time.deltaTime * 3);
    }

    void FindClosestEnemy()
    {
        if (levelSession == null)
        {
            Debug.LogError("Level Session for Player is NULL", this);
            return;
        }

        //closestEnemy = null;

        if(levelSession.currentActionPoint.actionPointEnemies.Count == 0)
        {
            closestEnemy = null;
            return;
        }

        foreach (var enemy in levelSession.currentActionPoint.actionPointEnemies)
        {
            if (enemy == null) continue;
            float dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (closestEnemy == null || dist < closestDist || closestEnemy.dead)
            {
                closestDist = dist;
                closestEnemy = enemy;
            }
        }
    }


    private void OnDestroy()
    {
        if(_inputService != null)
        {
            _inputService.OnColliderClick -= GetThrowDirection;
            _inputService.OnNonColliderClick -= GetThrowDirection;
        }
        EventService.OnEnemyHit -= OnEnemyHit;
        EventService.OnHitEnemyHead -= OnHitEnemyHead;
        EventService.OnGameOver -= OnGameOver;
    }

    public override void TakeDamage(float dmg, Vector3 dir, EDamageType damageType)
    {
        if (health.heathPoint <= 0) return;

        health.heathPoint -= dmg;

        if (health.heathPoint <= 0)
        {
            ChangeState(EPlayerState.Death);
        }
    }
}

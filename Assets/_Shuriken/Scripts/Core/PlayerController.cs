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

    public enum EPlayerState
    {
        MoveToPoint,
        Stand,
        Death
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

    void Awake()
    {
        animator = GetComponent<Animator>();
        EventService.OnEnemyHit += OnEnemyHit;
        EventService.OnHitEnemyHead += OnHitEnemyHead;
    }

    private void OnEnemyHit()
    {
        Debug.LogError("Enemy HIT");
        if (powerThrow < 5)
        {
            powerThrow++;
            if (powerThrow >= 5)
            {
                powerThrow = 5;
                shotMega = true;
            }
        }
        
    }

    private void OnHitEnemyHead()
    {
        Debug.LogError("HEAD HIT");
        if (powerThrow < 5)
        {
            powerThrow += 3;
            if (powerThrow >= 5)
            {
                powerThrow = 5;
                shotMega = true;
            }
        }

    }

    void Start()
    {
        //MoveToPoint(runPoint);
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

    void CheckDistanceToCurrentRunPoint()
    {
        if (runPoint == null) return;
        if (Vector3.Distance(transform.position, runPoint.position) < 0.5f)
        {
            ChangeState(EPlayerState.Stand);
        }
    }
 
    void StateMachine()
    {
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
                navMeshAgent.isStopped = true;
                blockShot = false;
                animator.ResetTrigger("ThrowR");
                animator.ResetTrigger("ThrowL");
                animator.ResetTrigger("ThrowR");
                animator.SetBool("Run", false);
                animator.CrossFade("Idle", 0.1f);
                break;
            case EPlayerState.Death:
                navMeshAgent.isStopped = true;
                animator.CrossFade("Death", 0.1f);
                EventService.OnPlayerDead?.Invoke();
                dead = true;
                break;
        }
    }

    public void MoveToPoint(Transform point)
    {
        //return;
        runPoint = point;
        navMeshAgent.SetDestination(runPoint.position);
        animator.SetBool("Run", true);
        animator.CrossFade("Run", 0.1f);
        ChangeState(EPlayerState.MoveToPoint);
    }


  
    void GetThrowDirection(Vector3 point)
    {
        throwTarget = null;
        throwPoint = transform.position + (point * 10) + (Vector3.up * heightMultiply);


        Debug.DrawLine(R_shurikenSpawnPos.position, throwPoint, Color.white, 3f);
        Debug.DrawLine(L_shurikenSpawnPos.position, throwPoint, Color.white, 3f);

        ThrowAnimation(false);
    }

    
    void GetThrowDirection(Vector3 point,Collider goCollider)
    {
        //if (playerState != EPlayerState.Stand) return;
        targetCol = goCollider;

        if (targetCol.transform.root.GetComponent<Pawn>().dead) targetCol = null;

        throwPoint = point;

        relPoint = R_shurikenSpawnPos.InverseTransformPoint(point);

        Debug.DrawLine(R_shurikenSpawnPos.position, point, Color.blue, 3f);


        ThrowAnimation();
    }

    void ThrowAnimation(bool nullTarget = false)
    {
        if(shotMega)
        {
            if (!blockShot)
            {
                animator.SetTrigger("ThrowM");
                blockShot = true;
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
            throwableObject = Instantiate(greatWeaponPrefab, R_shurikenSpawnPos.position, Quaternion.identity) as IThrowable;
            shotMega = false;
            powerThrow = 0;
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
    }

    public override void TakeDamage(float dmg)
    {
        if (health.heathPoint <= 0) return;

        health.heathPoint -= dmg;

        if (health.heathPoint <= 0)
        {
            ChangeState(EPlayerState.Death);
        }
    }
}

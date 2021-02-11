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


    public Shuriken shurikenPrefab;
    IThrowable throwableObject;

    public float heightMultiply;

    public enum EPlayerState
    {
        MoveToPoint,
        Stand
    }

    bool shotLeft;
    bool blockShot;

    IInputService _inputService;
    LevelSessionService levelSession;
    [Inject]
    void Construct(IInputService inputService)
    {
        _inputService = inputService;
        _inputService.OnColliderClick += GetThrowDirection;
        _inputService.OnNonColliderClick += GetThrowDirection;
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
        if (Vector3.Distance(transform.position, runPoint.position) < 0.2f)
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
                navMeshAgent.speed = 4;
              
                break;
            case EPlayerState.Stand:
                blockShot = false;
                animator.ResetTrigger("ThrowR");
                animator.ResetTrigger("ThrowL");
                animator.SetBool("Run", false);
                animator.CrossFade("Idle", 0.2f);
                break;
        }
    }

    public void MoveToPoint(Transform point)
    {
        //return;
        runPoint = point;
        navMeshAgent.SetDestination(runPoint.position);
        animator.SetBool("Run", true);
        animator.CrossFade("Run", 0.2f);
        ChangeState(EPlayerState.MoveToPoint);
    }


  
    void GetThrowDirection(Vector3 point)
    {
        throwTarget = null;
        throwPoint = transform.position + (point * 10) + (Vector3.up * heightMultiply);


        Debug.DrawLine(R_shurikenSpawnPos.position, throwPoint, Color.white, 3f);
        Debug.DrawLine(L_shurikenSpawnPos.position, throwPoint, Color.white, 3f);

        if (shotLeft)
        {
            if (!blockShot)
            {
                animator.SetTrigger("ThrowR");
                blockShot = true;
            }


        }
        else
        {
            if (!blockShot)
            {
                animator.SetTrigger("ThrowL");
                blockShot = true;
            }

        }


    }

    GameObject throwTarget;

    void GetThrowDirection(Vector3 point,GameObject go)
    {
        //if (playerState != EPlayerState.Stand) return;
        throwTarget = go;
        throwPoint = point;

        relPoint = R_shurikenSpawnPos.InverseTransformPoint(point);

        Debug.DrawLine(R_shurikenSpawnPos.position, point, Color.blue, 3f);
        //animator.SetTrigger("ThrowR");

        if (shotLeft)
        {
            if (!blockShot)
            {
                animator.SetTrigger("ThrowR");
                blockShot = true;
            }
               

        }
        else
        {
            if (!blockShot)
            {
                animator.SetTrigger("ThrowL");
                blockShot = true;
            }
                
        }
    }

    public void ThrowShurikenByAnimator()
    {
        
        ThrowShuriken(relPoint);
        //Debug.DrawLine(R_shurikenSpawnPos.transform.position, relPoint, Color.cyan, 2);
    }

    void ThrowShuriken(Vector3 relPoint)
    {
        if (shotLeft)
        {
            throwableObject = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
        }
        else
        {
           
            throwableObject = Instantiate(shurikenPrefab, L_shurikenSpawnPos.position, Quaternion.identity);
        }
        shotLeft = !shotLeft;
        blockShot = false;

        if (throwTarget != null)
        {
            throwableObject.SetMoveType(Shuriken.EMoveType.Target);
        }
        else
        {
            throwableObject.SetMoveType(Shuriken.EMoveType.Free);
        }
        throwableObject.SetTargetPosition(throwPoint);

        
        //throwableObject.SetMoveDirection(throwPoint);

    }

  

    void FindClosestEnemy()
    {
        if (levelSession == null)
        {
            Debug.LogError("Level Session for Player is NULL", this);
            return;
        }

        closestEnemy = null;

        foreach (var enemy in levelSession.currentActionPoint.actionPointEnemies)
        {
            if (enemy == null) continue;
            float dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (closestEnemy == null || dist < closestDist)
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
      
    }
}

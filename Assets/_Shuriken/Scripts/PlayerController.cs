using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerController : Pawn
{
    public Vector3 shurikenOffset;
    public SimpleMover shurikenPrefab;

    public Transform R_shurikenSpawnPos;
    public Transform L_shurikenSpawnPos;
    public Vector3 throwDirection;

    public Vector3 relPoint;

    public Transform runPoint;

    public EPlayerState playerState;

    public AIEnemy closestEnemy;
    public float closestDist;

    public enum EPlayerState
    {
        MoveToPoint,
        Stand
    }

    IInputService _inputService;
    LevelSessionService levelSession;
    [Inject]
    void Construct(IInputService inputService)
    {
        _inputService = inputService;
        _inputService.OnColliderClick += GetThrowDirection;
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
                break;
            case EPlayerState.Stand:
                animator.SetBool("Run", false);
                break;
        }
    }

    public void MoveToPoint(Transform point)
    {
        runPoint = point;
        navMeshAgent.SetDestination(runPoint.position);
        animator.SetBool("Run", true);
        ChangeState(EPlayerState.MoveToPoint);
    }

    /*
    void GetThrowDirection(Vector3 point)
    {
        throwDir = point;
        // throwDir = point - (shurikenSpawnPos.position);
        Vector3 crossRes = Vector3.Cross(transform.position, point);
        direction = Vector3.Dot(crossRes, Vector3.up);

        if(direction > 0)
        {
            animator.SetTrigger("ThrowR");
        }
        else if(direction < 0){
            animator.SetTrigger("ThrowL");
        }
        else
        {
            animator.SetTrigger("ThrowR");
        }
       
    }*/

    void GetThrowDirection(Vector3 point,GameObject go)
    {
        if (playerState != EPlayerState.Stand) return;

        throwDir = point;

        relPoint = transform.InverseTransformPoint(point);


        if (relPoint.x > 1)
            animator.SetTrigger("ThrowL");
        else if (relPoint.x < -1)
            animator.SetTrigger("ThrowR");
        else
            animator.SetTrigger("ThrowM");

    }

    public Vector3 throwDir;

    public void ThrowShurikenByAnimator()
    {
        ThrowShuriken(relPoint);
    }

    void ThrowShuriken(Vector3 relPoint)
    {
        SimpleMover shuriken;
      
        /*
        if (relPoint.x >= 0)
        {
            shuriken = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Right;
        }
        else
        {
            shuriken = Instantiate(shurikenPrefab, L_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Left;
        }*/

        if (relPoint.x > 1)
        {
            shuriken = Instantiate(shurikenPrefab, L_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Right;
            Debug.Log("Throw Right");
        }
        else if (relPoint.x < -1)
        {
            shuriken = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Left;
            Debug.Log("Throw Left");
        }
        else
        {
            shuriken = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
            shuriken.flySide = SimpleMover.EFlySide.Middle;
            //shuriken.rotateDir = Vector3.zero;

            shuriken.transform.Rotate(0, 0, Random.Range(-180,180));
           // shuriken.rotateDir = Vector3.left * 20;
        }

        if(shuriken != null)
            shuriken.SetTargetPosition(throwDir);
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
        _inputService.OnColliderClick -= GetThrowDirection;
    }
}

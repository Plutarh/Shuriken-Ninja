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
    public Vector3 throwDir;
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
                break;
            case EPlayerState.Stand:
                animator.SetBool("Run", false);
                animator.CrossFade("Idle", 0.2f);
                break;
        }
    }

    public void MoveToPoint(Transform point)
    {
       
        runPoint = point;
        navMeshAgent.SetDestination(runPoint.position);
        animator.SetBool("Run", true);
        animator.CrossFade("Run", 0.2f);
        ChangeState(EPlayerState.MoveToPoint);
    }

    
    void GetThrowDirection(Vector3 point)
    {

        throwDir = transform.position + (point * 10) + (Vector3.up * 2);

        Debug.DrawLine(R_shurikenSpawnPos.position, R_shurikenSpawnPos.position + (point * 10) + Vector3.up, Color.white, 3f);

        animator.SetTrigger("ThrowR");
    }

    void GetThrowDirection(Vector3 point,GameObject go)
    {
        if (playerState != EPlayerState.Stand) return;

        throwDir = point;

        relPoint = transform.InverseTransformPoint(point);
        animator.SetTrigger("ThrowR");

        return;
        if (relPoint.x > 1)
            animator.SetTrigger("ThrowL");
      
           
        else
            animator.SetTrigger("ThrowM");

    }

   

    public void ThrowShurikenByAnimator()
    {
        ThrowShuriken(relPoint);
    }

    void ThrowShuriken(Vector3 relPoint)
    {
        SimpleMover shuriken;
        shuriken = Instantiate(shurikenPrefab, R_shurikenSpawnPos.position, Quaternion.identity);
        shuriken.flySide = SimpleMover.EFlySide.Right;



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
        if(_inputService != null)
        {
            _inputService.OnColliderClick -= GetThrowDirection;
            _inputService.OnNonColliderClick -= GetThrowDirection;
        }
      
    }
}

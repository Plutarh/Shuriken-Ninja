using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActionPoint : MonoBehaviour
{
    public PawnSpawner pawnSpawner;
    public Transform playerActionPosition;

    public EActionPointState pointState;

    public List<AIEnemy> actionPointEnemies = new List<AIEnemy>();

    public int enemiesCount;

    public enum EActionPointState
    {
        Wait,
        Action,
        Done
    }

    LevelSessionService _levelSessionService;

    [Inject]
    void Construct(LevelSessionService levelSessionService)
    {
        _levelSessionService = levelSessionService;
        _levelSessionService.actionPoints.Add(this);
    }

    private void Awake()
    {
        pawnSpawner.OnPawnSpawn += AddEnemy;

        pawnSpawner.spawnCount = enemiesCount;
    }

    void Start()
    {
        
    }

    void Update()
    {
        StateLogic();
    }

    void StateLogic()
    {
        switch (pointState)
        {
            case EActionPointState.Wait:
                break;
            case EActionPointState.Action:
                pawnSpawner.SpawnPawn();
                
                break;
            case EActionPointState.Done:
                break;
        }
    }

    public void ChangeState(EActionPointState newState)
    {
        if (newState == pointState) return;
        pointState = newState;
    }

    void AddEnemy(AIEnemy pawn)
    {
        if (!actionPointEnemies.Contains(pawn))
        {
            actionPointEnemies.Add(pawn);
            pawn.OnDeath += RemoveEnemy;
        }
            
    }

    void RemoveEnemy(AIEnemy enemy)
    {
        if (actionPointEnemies.Contains(enemy))
        {
            enemy.OnDeath -= RemoveEnemy;
            actionPointEnemies.Remove(enemy);
            enemiesCount--;
        }

        if(enemiesCount <= 0)
        {
            ChangeState(EActionPointState.Done);
            _levelSessionService.ActionPointDone(this);
        }
    }

    private void OnDestroy()
    {
        pawnSpawner.OnPawnSpawn -= AddEnemy;
    }
}

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
    public enum EActionPointState
    {
        Wait,
        Action,
        Done
    }

    [Inject]
    void Construct(LevelSessionService levelSessionService)
    {
        levelSessionService.actionPoints.Add(this);
    }

    private void Awake()
    {
        pawnSpawner.OnPawnSpawn += AddPawn;
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

    void AddPawn(AIEnemy pawn)
    {
        actionPointEnemies.Add(pawn);
        if (!actionPointEnemies.Contains(pawn))
            actionPointEnemies.Add(pawn);
    }

    private void OnDestroy()
    {
        pawnSpawner.OnPawnSpawn -= AddPawn;
    }
}

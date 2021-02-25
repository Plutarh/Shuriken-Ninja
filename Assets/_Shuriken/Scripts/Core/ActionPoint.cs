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

    public float livePawns;
    public float spawnCount;
    public float spawnDelay;

    float spawnTimer;

    public ParticleSystem auraFX;
    public ParticleSystem spawnFX;

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
        if(pawnSpawner != null)
        {
            pawnSpawner.OnPawnSpawn += AddEnemy;
            auraFX.gameObject.SetActive(false);
        }

        EventService.OnPlayerWakedUp += DestroyLiveEnemies;


    }

    void Start()
    {
        
    }

    void Update()
    {
        StateLogic();
    }

    void DestroyLiveEnemies()
    {
        actionPointEnemies.ForEach(ape => ape.Disappear());
        actionPointEnemies.Clear();
        livePawns = 0;
        spawnCount = 2;
        spawnTimer = spawnDelay;
    }
    void StateLogic()
    {
        switch (pointState)
        {
            case EActionPointState.Wait:
                break;
            case EActionPointState.Action:
                if(pawnSpawner != null)
                {
                    if (spawnCount <= 0) return;
                    spawnTimer += Time.deltaTime;
                    if (spawnTimer > spawnDelay)
                    {
                        if (!spawnFX.isPlaying) spawnFX.Play();
                        pawnSpawner.SpawnPawn();
                        spawnTimer = 0;
                        spawnCount--;
                        livePawns++;
                    }
                }
                break;
            case EActionPointState.Done:
              
                break;
        }
    }

    public void ChangeState(EActionPointState newState)
    {
        if (newState == pointState) return;
        pointState = newState;

        switch (pointState)
        {
            case EActionPointState.Wait:
                break;
            case EActionPointState.Action:
                auraFX.gameObject.SetActive(true);
                if (!auraFX.isPlaying) auraFX.Play();
                break;
            case EActionPointState.Done:
                if (auraFX.isPlaying) auraFX.Stop();
                break;
        }
    }

    int priorityIndex = 0;

    void AddEnemy(AIEnemy pawn)
    {
        if (!actionPointEnemies.Contains(pawn))
        {
            pawn.navMeshAgent.avoidancePriority = priorityIndex;
            actionPointEnemies.Add(pawn);
            pawn.OnDeath += RemoveEnemy;
            priorityIndex++;
        }
            
    }

    void RemoveEnemy(AIEnemy enemy)
    {
        if (actionPointEnemies.Contains(enemy))
        {
            enemy.OnDeath -= RemoveEnemy;
            actionPointEnemies.Remove(enemy);
            livePawns--;
        }

        if(livePawns <= 0 && spawnCount <= 0)
        {
            ChangeState(EActionPointState.Done);
            _levelSessionService.ActionPointDone(this);
        }
    }

    private void OnDestroy()
    {
        if(pawnSpawner != null)
            pawnSpawner.OnPawnSpawn -= AddEnemy;

        EventService.OnPlayerWakedUp -= DestroyLiveEnemies;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawCube(playerActionPosition.position, Vector3.one);
    }
}

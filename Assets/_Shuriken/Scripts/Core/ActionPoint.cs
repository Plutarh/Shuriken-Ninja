using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActionPoint : MonoBehaviour
{
    public PawnSpawner pawnSpawner;
    public Transform playerActionPosition;
    public List<Transform> spawnPositions = new List<Transform>();
    public EActionPointState pointState;

    public List<AIEnemy> actionPointEnemies = new List<AIEnemy>();

    public float livePawns;
    public float spawnCount;
    public float spawnDelay;
    float _spawnCount;

    float spawnTimer;

    public int spawnPosIndex;

    public Transform spawnPointsParent;

    [Header("FX")]
    public ParticleSystem auraFX;
    public ParticleSystem spawnFX;

    public List<ParticleSystem> spawnPosAuraParcticles = new List<ParticleSystem>();
    public List<ParticleSystem> spawnPosSpawnParcticles = new List<ParticleSystem>();

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

    private void OnValidate()
    {
        foreach (Transform sp in spawnPointsParent)
        {
            if (!spawnPositions.Contains(sp)) spawnPositions.Add(sp);
        }

        for (int i = 0; i < spawnPositions.Count; i++)
        {
            if (spawnPositions[i] == null) spawnPositions.Remove(spawnPositions[i]);
        }
    }

    private void Awake()
    {
        _spawnCount = spawnCount;
        if (pawnSpawner != null)
        {
            pawnSpawner.OnPawnSpawn += AddEnemy;
          
        }

        EventService.OnPlayerWakedUp += DestroyLiveEnemies;

        foreach (var sp in spawnPositions)
        {
            var createdAuraFX = Instantiate(auraFX, sp);
            var createdSpawnFX = Instantiate(spawnFX, sp);
            createdSpawnFX.gameObject.SetActive(false);
            spawnPosAuraParcticles.Add(createdAuraFX);
            spawnPosSpawnParcticles.Add(createdSpawnFX);
        }
        EnableSpawnAuraPacticle(false);
       
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
        spawnCount = _spawnCount;
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
                        if (spawnPosIndex > spawnPositions.Count - 1) spawnPosIndex = 0;
                        
                        pawnSpawner.SpawnPawn(spawnPositions[spawnPosIndex]);
                        ShowSpawnPartciles(spawnPosIndex);
                        spawnTimer = 0;
                        spawnCount--;
                        livePawns++;
                        spawnPosIndex++;
                    }
                }
                break;
            case EActionPointState.Done:
              
                break;
        }
    }

    void ShowSpawnPartciles(int index)
    {
        if (!spawnPosSpawnParcticles[index].gameObject.activeSelf) spawnPosSpawnParcticles[index].gameObject.SetActive(true);
        if (!spawnPosSpawnParcticles[index].isPlaying) spawnPosSpawnParcticles[index].Play();
    }

    void EnableSpawnAuraPacticle(bool enabled)
    {
        foreach (var spap in spawnPosAuraParcticles)
        {
            spap.gameObject.SetActive(enabled);
            if (enabled)
            {
                if (!spap.isPlaying) spap.Play();
            }
            else
            {
                if (spap.isPlaying) spap.Stop();
            }
        }
    }

    void DeactivateSpawnAuraPacticle()
    {
        foreach (var spap in spawnPosAuraParcticles)
        {
            if (spap.isPlaying) spap.Stop();
        }
    }

    public void ChangeState(EActionPointState newState)
    {
        if (newState == pointState) return;
        pointState = newState;

        switch (pointState)
        {
            case EActionPointState.Wait:
                EnableSpawnAuraPacticle(false);
                break;
            case EActionPointState.Action:
                spawnTimer = spawnDelay;
                EnableSpawnAuraPacticle(true);
                break;
            case EActionPointState.Done:
                DeactivateSpawnAuraPacticle();
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

        Gizmos.color = Color.yellow;

        foreach (var sp in spawnPositions)
        {
            Gizmos.DrawSphere(sp.position, 1);
        }

    }
}

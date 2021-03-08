using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ActionPoint : MonoBehaviour
{
    public PawnSpawner pawnSpawner;
    public Transform playerActionPosition;
    public List<Transform> spawnPositions = new List<Transform>();
    public EActionPointState pointState;

    
    public List<AIEnemy> actionPointEnemies = new List<AIEnemy>();
    public List<AIEnemy> stayedEnemies = new List<AIEnemy>();

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
        //_levelSessionService.actionPoints.Add(this);
    }

    private void OnValidate()
    {
        
    }

    private void Awake()
    {
        _spawnCount = spawnCount;
        if (pawnSpawner != null)
        {
            pawnSpawner.OnPawnSpawn += AddEnemy;
        }

        EventService.OnPlayerWakedUp += DestroyLiveEnemies;

        CreateSpawnerParctiles();
        //EnableSpawnAuraPacticle(false);
        CheckStayedEnemies();
    }

    void Start()
    {
        
    }

    void Update()
    {
        StateLogic();
    }

    void CheckStayedEnemies()
    {
        if (stayedEnemies.Count == 0) return;

        foreach (var se in stayedEnemies)
        {
            AddEnemy(se);
            livePawns++;
            se.transform.SetParent(null);
            //se.OnDeath += RemoveEnemy;
        }
        stayedEnemies.Clear();


    }
    
    public void FindSpawnPoints()
    {
        spawnPositions.Clear();
        foreach (Transform sp in spawnPointsParent)
        {
            if (!spawnPositions.Contains(sp)) spawnPositions.Add(sp);
        }
    }

    void CreateSpawnerParctiles()
    {
        foreach (var sp in spawnPositions)
        {
            var createdAuraFX = Instantiate(auraFX, sp);
            var createdSpawnFX = Instantiate(spawnFX, sp);
            createdSpawnFX.gameObject.SetActive(false);
            spawnPosAuraParcticles.Add(createdAuraFX);
            spawnPosSpawnParcticles.Add(createdSpawnFX);
        }
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
                        
                        AIEnemy spawnedEnemy = pawnSpawner.SpawnPawn(spawnPositions[spawnPosIndex]);
                        spawnedEnemy.transform.LookAt(_levelSessionService.player.transform);
                        ShowSpawnPartciles(spawnPosIndex);
                        spawnTimer = 0;
                        spawnCount--;
                        livePawns++;
                        spawnPosIndex++;
                        spawnedEnemy.spawnedByPoint = true;
                        //spawnedEnemy.ChangeState(AIEnemy.EAIState.Chaise);
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
        if(spawnPosAuraParcticles.Count == 0)
        {
            Debug.LogError("Spawn FX 0 count",this);
            return;
        }

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

                // Отправляем в бой врагов,которые были не заспавлены, а уже стояли на месте
                actionPointEnemies.Where(ape => !ape.spawnedByPoint)
                    .ToList()
                    .ForEach(enemy => enemy.ChangeState(AIEnemy.EAIState.Chaise));

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
        else if (stayedEnemies.Contains(enemy))
        {
            enemy.OnDeath -= RemoveEnemy;
            stayedEnemies.Remove(enemy);
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

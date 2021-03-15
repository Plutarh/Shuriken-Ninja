using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class LevelSessionService : MonoBehaviour
{

    public PlayerController player;

    public List<ActionPoint> actionPoints = new List<ActionPoint>();
    

    public ActionPoint currentActionPoint;
    public int curActionPointIndex;

    public float delayToStartNextPoint;
    public ELevelState levelState;

    [Header("Level Statistic")]
    public int totalEnemyCount;
    public int deadEnemyCount;
    public float completeLevelProgress;

    public enum ELevelState
    {
        NotStarted,
        Proccess,
        Finish
    }

    [Inject]
    void Construct(PlayerController playerC)
    {
        player = playerC;
        player.SetLevelSessionService(this);
    }

    private void OnValidate()
    {
       
    }

    private void Awake()
    {
        levelState = ELevelState.NotStarted;
       
        SetCurrentActionPoint();

        EventService.OnTapToPlay += StartGame;
        EventService.OnPlayerDead += OnPlayerDead;
        EventService.OnPlayerRanActionPoint += InvokeActionPoint;
        EventService.OnEnemyDeath += EnemyDeathCounter;

        EventService.OnUpdateLevelKillStatistic?.Invoke(0.03f);
    }

    void Start()
    {
        //SetPlayerNextMovePoint();
        CheckActionPointsEnemyCount();
    }

    void Update()
    {

    }

    void CheckActionPointsEnemyCount()
    {
        foreach (var ap in actionPoints)
        {
            totalEnemyCount += ap.actionPointEnemies.Count;
            totalEnemyCount += (int)ap.spawnCount;
        }
    }

    void StartGame()
    {
        levelState = ELevelState.Proccess;
        SetPlayerNextMovePoint();
    }

    void OnPlayerDead()
    {
        EventService.OnGameOver?.Invoke(EventService.EGameState.Loose);
    }

    void EnemyDeathCounter(AIEnemy deathEnemy)
    {
        deadEnemyCount++;

        completeLevelProgress = ((float)deadEnemyCount / (float)totalEnemyCount);

        EventService.OnUpdateLevelKillStatistic?.Invoke(completeLevelProgress);
    }

    private void SetPlayerNextMovePoint()
    {
        StartCoroutine(IESetPlayerNextMovePoint());
    }

    void InvokeActionPoint()
    {
        Debug.Log($"Invoke {currentActionPoint.name}");
        currentActionPoint.ChangeState(ActionPoint.EActionPointState.Action);
    }

    IEnumerator IESetPlayerNextMovePoint()
    {
        float delay = delayToStartNextPoint;
        if (curActionPointIndex == 0) delay = 0;
        yield return new WaitForSecondsRealtime(delay);
      
        player.MoveToPoint(currentActionPoint.playerActionPosition);
    }

  
    void SetCurrentActionPoint()
    {
        if (actionPoints.Count > 0 && curActionPointIndex < actionPoints.Count)
        {
            currentActionPoint = actionPoints[curActionPointIndex];
        }
        else
        { 
            Debug.LogError("Нету такого Action Point", this);
            StartCoroutine(IEWaitToWin());
        }
           
    }

    IEnumerator IEWaitToWin()
    {
        yield return new WaitForSecondsRealtime(2);
        EventService.OnGameOver?.Invoke(EventService.EGameState.Win);
    }

    public void ActionPointDone(ActionPoint actionPoint)
    {
        curActionPointIndex++;
        SetCurrentActionPoint();
        SetPlayerNextMovePoint();
    }

    private void OnDestroy()
    {
        EventService.OnTapToPlay -= StartGame;
        EventService.OnPlayerDead -= OnPlayerDead;
        EventService.OnPlayerRanActionPoint -= InvokeActionPoint;
        EventService.OnEnemyDeath -= EnemyDeathCounter;
    }

#if UNITY_EDITOR
    public void FindAllActionPoints()
    {
        actionPoints.Clear();
        var foundedActionPoints = FindObjectsOfType<ActionPoint>().ToList();
        var sortedPoints = foundedActionPoints.OrderBy(p => p.name);

        
        sortedPoints.ToList().ForEach(sp => actionPoints.Add(sp));
    }

    public void FindActionPointEnemies()
    {
        foreach (var ap in actionPoints)
        {
            ap.stayedEnemies.Clear();
            ap.stayedEnemies = ap.transform.GetComponentsInChildren<AIEnemy>().ToList();
            ap.FindSpawnPoints();
            UnityEditor.EditorUtility.SetDirty(ap);
        }
    }
#endif
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(LevelSessionService))]
public class LevelSessionServiceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelSessionService myScript = (LevelSessionService)target;
        if (GUILayout.Button("Setup all action points"))
        {
            myScript.FindAllActionPoints();
            myScript.FindActionPointEnemies();

            UnityEditor.EditorUtility.SetDirty(myScript);
            //UnityEditor.EditorSceneManager.MarkSceneDirty(myScript.gameObject.scene);
        }
    }
}

#endif

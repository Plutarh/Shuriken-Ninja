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
    }

    void Start()
    {
        //SetPlayerNextMovePoint();
    }

    void Update()
    {

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
    }

    public void FindAllActionPoints()
    {
        actionPoints.Clear();
        actionPoints = FindObjectsOfType<ActionPoint>().ToList();
        var sortedPoints = actionPoints.OrderBy(p => p.name);
        actionPoints = sortedPoints.ToList();
    }

    public void FindActionPointEnemies()
    {
        foreach (var ap in actionPoints)
        {
            ap.stayedEnemies.Clear();
            ap.stayedEnemies = ap.transform.GetComponentsInChildren<AIEnemy>().ToList();
            ap.FindSpawnPoints();
        }
    }
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
        }
    }
}

#endif

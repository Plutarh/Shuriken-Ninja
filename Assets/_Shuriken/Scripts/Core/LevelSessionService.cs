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
    void Constuct(PlayerController playerC)
    {
        player = playerC;
        player.SetLevelSessionService(this);
    }

    private void OnValidate()
    {
        /*
        if(actionPoints.Count == 0)
        {
            actionPoints = FindObjectsOfType<ActionPoint>().ToList();
        }*/
    }

    private void Awake()
    {
        levelState = ELevelState.NotStarted;
        var sortedPoints = actionPoints.OrderBy(p => p.name);
        actionPoints = sortedPoints.ToList();
        SetCurrentActionPoint();

        EventService.OnTapToPlay += StartGame;
        EventService.OnPlayerDead += OnPlayerDead;
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

    IEnumerator IESetPlayerNextMovePoint()
    {
        float delay = delayToStartNextPoint;
        if (curActionPointIndex == 0) delay = 0;
        yield return new WaitForSecondsRealtime(delay);
        currentActionPoint.ChangeState(ActionPoint.EActionPointState.Action);
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
    }
}

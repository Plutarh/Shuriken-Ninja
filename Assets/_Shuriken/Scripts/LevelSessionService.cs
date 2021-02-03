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

    [Inject]
    void Constuct(PlayerController playerC)
    {
        player = playerC;
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
        SetCurrentActionPoint();
    }

    void Start()
    {
        SetPlayerMovePoint();
    }

    private void SetPlayerMovePoint()
    {
        currentActionPoint.ChangeState(ActionPoint.EActionPointState.Action);
        player.MoveToPoint(currentActionPoint.playerActionPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetCurrentActionPoint()
    {
        if (actionPoints.Count > 0 && curActionPointIndex < actionPoints.Count - 1)
            currentActionPoint = actionPoints[curActionPointIndex];
    }
}

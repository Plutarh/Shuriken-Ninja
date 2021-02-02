using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPoint : MonoBehaviour
{
    public PawnSpawner pawnSpawner;
    public Transform playerActionPosition;

    public EActionPointState pointState;
    
    public enum EActionPointState
    {
        Wait,
        Action,
        Done
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

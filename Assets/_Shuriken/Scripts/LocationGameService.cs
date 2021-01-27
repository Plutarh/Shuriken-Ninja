using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LocationGameService : MonoBehaviour
{

    public PlayerController player;

    

    [Inject]
    void Construct(PlayerController playerController)
    {
        player = playerController;
        Debug.Log("Get player");
    }
    private void Awake()
    {

    }
    void Start()
    {

    }

    void Update()
    {
        
    }

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public float coins;
    public int totalLevelWin;

    public static PlayerData Get;

    private void Awake()
    {
        EventService.OnEnemyDeath += AddMoneyForEnemyKill;
        Get = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    void AddMoneyForEnemyKill(AIEnemy deathEnemy)
    {
        float reward = 5;
        coins += reward;
        EventService.OnChangePlayerCoinsCount?.Invoke(reward);
    }

    private void OnDestroy()
    {
        EventService.OnEnemyDeath -= AddMoneyForEnemyKill;
    }
}

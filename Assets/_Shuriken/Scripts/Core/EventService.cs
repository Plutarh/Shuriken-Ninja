using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventService
{

    public static Action OnHitEnemyHead;
    public static Action OnEnemyHit;
    public static Action<AIEnemy> OnEnemyDeath;
    public static Action OnPlayerDead;
    public static Action OnPlayerRanActionPoint;
    public static Action OnTapToPlay;
    public static Action OnNewSceneLoaded;
    public static Action OnPlayerWakedUp;
    public static Action<EGameState> OnGameOver;
    public static Action<float> OnUpdateLevelKillStatistic;
    public static Action<float> OnChangePlayerCoinsCount;

    public enum EGameState
    {
        Win,
        Loose,
        Continue
    }
}

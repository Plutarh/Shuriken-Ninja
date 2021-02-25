using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventService
{

    public static Action OnHitEnemyHead;
    public static Action OnEnemyHit;
    public static Action OnPlayerDead;
    public static Action OnTapToPlay;
    public static Action OnNewSceneLoaded;
    public static Action<EGameState> OnGameOver;

    public enum EGameState
    {
        Win,
        Loose
    }
}

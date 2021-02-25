using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public static Core Get;


    [SerializeField] SceneLoader sceneLoader;

    private void Awake()
    {
        Get = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TapToPlay()
    {
        EventService.OnTapToPlay?.Invoke();
    }

    public void RestartLevel()
    {
        sceneLoader.RestartLevel();
    }

    public void ContinueLevel()
    {
        EventService.OnGameOver?.Invoke(EventService.EGameState.Continue);
    }

    public void NextLevel()
    {
        sceneLoader.LoadNextLevel();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public int sceneIndex;
    Coroutine loadRoutine;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartLevel()
    {
        LoadSceneByName(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex > SceneManager.sceneCount + 1)
        {
            sceneIndex = 0;
            return;
        }
        sceneIndex++;
        LoadSceneByIndex(sceneIndex);
    }

    public void LoadPrevLevel()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex - 1 < 0) return;
        sceneIndex--;
        LoadSceneByIndex(sceneIndex);
    }

    void LoadSceneByName(string sceneName)
    {
        StartCoroutine(IELoadSceneRoutine(sceneName));
    }

    void LoadSceneByIndex(int sceneIndex)
    {
        StartCoroutine(IELoadSceneRoutine(null, sceneIndex));
    }


    IEnumerator IELoadSceneRoutine(string sceneName = null,int sceneIndex = -1)
    {
        AsyncOperation async = null;
        if (sceneName != null)
        {
            async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        else if(sceneIndex != -1)
        {
            async = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        }
    

        async.allowSceneActivation = false;
        if (async == null)
        {
            Debug.LogError("Cannot Load scene !!!");
            yield break;
        }

        while (!async.isDone)
        {
            if (async.progress >= 0.9f || async.isDone)
            {

                Debug.LogError("Loaded scene !!!");
                async.allowSceneActivation = true;
                EventService.OnNewSceneLoaded?.Invoke();
            }
            yield return null;
        }

    }
}

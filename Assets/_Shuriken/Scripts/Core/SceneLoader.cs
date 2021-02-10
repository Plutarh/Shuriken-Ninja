using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public int sceneIndex;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex + 1 > SceneManager.sceneCount + 1) return;
        SceneManager.LoadScene(sceneIndex + 1);
    }

    public void LoadPrevLevel()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex - 1 < 0) return;
        SceneManager.LoadScene(sceneIndex - 1);
    }
}

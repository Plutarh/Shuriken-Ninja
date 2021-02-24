using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Get;

    public UIPanel tapToPlayPanel;
    public Image playerThrowPower;

    private void Awake()
    {
        Get = this;
        EventService.OnNewSceneLoaded += OnNewSceneLoaded;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnNewSceneLoaded()
    {
        tapToPlayPanel.Show(true);
    }

    public void SetPlayerThrowPoint(int points)
    {
        playerThrowPower.fillAmount = (float)points / 5;
    }

    private void OnDestroy()
    {
        EventService.OnNewSceneLoaded -= OnNewSceneLoaded;
    }
}

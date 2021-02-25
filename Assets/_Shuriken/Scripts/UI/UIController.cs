using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Get;

    public Image playerThrowPower;
    [Header("Panels")]
    public UIPanel tapToPlayPanel;
    public UIPanel winPanel;
    public UIPanel loosePanel;


    public List<UIPanel> allPanels = new List<UIPanel>();

    private void Awake()
    {
        Get = this;
        EventService.OnNewSceneLoaded += OnNewSceneLoaded;
        EventService.OnGameOver += OnGameOver;

        OnNewSceneLoaded();
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
        foreach (var panel in allPanels)
        {
            panel.gameObject.SetActive(false);
        }
        tapToPlayPanel.Show(true);
    }

    public void SetPlayerThrowPoint(int points)
    {
        playerThrowPower.fillAmount = (float)points / 5;
    }

    void OnGameOver(EventService.EGameState gameState)
    {
        switch (gameState)
        {
            case EventService.EGameState.Win:
                winPanel.Show();
                break;
            case EventService.EGameState.Loose:
                loosePanel.Show();
                break;
            case EventService.EGameState.Continue:
                loosePanel.Hide();
                break;
        }
    }

    private void OnDestroy()
    {
        EventService.OnNewSceneLoaded -= OnNewSceneLoaded;
        EventService.OnGameOver -= OnGameOver;
    }


}

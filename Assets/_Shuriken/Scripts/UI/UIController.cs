using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public static UIController Get;

    public Image playerThrowPower;
    [Header("Panels")]
    public UIPanel tapToPlayPanel;
    public UIPanel winPanel;
    public UIPanel loosePanel;


    public List<UIPanel> allPanels = new List<UIPanel>();

    public Slider levelProgressSlider;
    public Image finishImg;
    public Text levelNumberText;

    private void Awake()
    {
        Get = this;
        EventService.OnNewSceneLoaded += OnNewSceneLoaded;
        EventService.OnGameOver += OnGameOver;
        EventService.OnUpdateLevelKillStatistic += UpdateLevelProgressSlider;
        EventService.OnNewSceneLoaded += UpdateLevelNumberText;

        OnNewSceneLoaded();
    }

    void Start()
    {
        UpdateLevelNumberText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateLevelNumberText()
    {
        levelNumberText.text = SceneLoader.Get.sceneIndex.ToString();
    }

    void UpdateLevelProgressSlider(float val)
    {
        //levelProgressSlider.value = val;


        if (val < levelProgressSlider.value)
        {
            finishImg.transform.localScale = Vector3.one;
            levelProgressSlider.value = val;
            return;
        }

        DOTween.To(() => levelProgressSlider.value, x => levelProgressSlider.value = x, val, 0.15f).SetUpdate(true).OnComplete(() =>
        {
            if(val >= 1)
            {
                finishImg.transform.DOScale(Vector3.one * 1.3f, 1f).SetEase(Ease.InOutBack);
            }
           
        });
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
        EventService.OnUpdateLevelKillStatistic -= UpdateLevelProgressSlider;
        EventService.OnNewSceneLoaded -= UpdateLevelNumberText;
    }


}

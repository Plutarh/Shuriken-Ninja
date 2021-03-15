using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerCoinsPanel : MonoBehaviour
{

    public Text playerCoinsText;

    float playerCoinsUI;

    private void Awake()
    {
        UpdatePlayerCoinsText();
        EventService.OnChangePlayerCoinsCount += UpdatePlayerCoinsText;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void UpdatePlayerCoinsText()
    {
        playerCoinsText.text = PlayerData.Get.coins.ToString();
    }

    void UpdatePlayerCoinsText(float val)
    {
        playerCoinsUI = (PlayerData.Get.coins - val);

        DOTween.To(() => playerCoinsUI, x => playerCoinsUI = x, PlayerData.Get.coins, 0.5f).SetUpdate(true).OnUpdate(() =>
        {
            playerCoinsText.text = playerCoinsUI.ToString("#");
        });
    }

    private void OnDestroy()
    {
        EventService.OnChangePlayerCoinsCount -= UpdatePlayerCoinsText;
    }
}

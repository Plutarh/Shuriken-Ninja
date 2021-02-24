using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Get;


    public Image playerThrowPower;

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

    public void SetPlayerThrowPoint(int points)
    {
        playerThrowPower.fillAmount = (float)points / 5;
    }
}

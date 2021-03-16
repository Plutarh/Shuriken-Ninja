using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class OptimizationService : MonoBehaviour
{
    public UniversalRenderPipelineAsset urpAsset;

    public Text DPItext;

    public PostProcessData postProcess;

    public Slider mainSlider;
    public Slider fpsSlider;



    void Start()
    {
        var rpAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        urpAsset = (UniversalRenderPipelineAsset)rpAsset;

#if UNITY_EDITOR
        urpAsset.renderScale = 1;

#elif UNITY_ANDROID && !UNITY_EDITOR
        urpAsset.renderScale = 0.5f;
#endif

        
    }

    // Update is called once per frame
    void Update()
    {
       // DPItext.text = Screen.currentResolution.ToString();
    }

    public void ChangeRenderScale()
    {
        return;
        urpAsset.renderScale = mainSlider.value;
       
    }

    public void HDRP()
    {
        urpAsset.supportsHDR = !urpAsset.supportsHDR;

       
    }


    public void SRPBatch()
    {
        urpAsset.useSRPBatcher = !urpAsset.useSRPBatcher;
    }

    public void PP()
    {
        FindObjectOfType<Volume>().enabled = !FindObjectOfType<Volume>().enabled;
    }

    public void Fps()
    {
        return;
        Application.targetFrameRate = (int)fpsSlider.value;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OptimizationService : MonoBehaviour
{
    public UniversalRenderPipelineAsset urpAsset;

    void Start()
    {
        var rpAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        urpAsset = (UniversalRenderPipelineAsset)rpAsset;

#if UNITY_EDITOR
        urpAsset.renderScale = 1;

#elif UNITY_ANDROID && !UNITY_EDITOR
        urpAsset.renderScale = 0.7f;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        // scaleText.text = urpAsset.name + " - " + urpAsset.renderScale;
    }
}

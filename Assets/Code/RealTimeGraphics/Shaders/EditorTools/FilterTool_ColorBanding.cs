using System;
using UnityEngine;

public class FilterTool_ColorBanding : FilterTool_Abs
{
    private static readonly int ColorStepID = Shader.PropertyToID("_ColorSteps");
    [SerializeField] private int colorSteps;

    public override void StartUpActivity()
    {
        rendererFeature.SetActive(activeState);
        fullScreenMat.SetFloat(ColorStepID,colorSteps);
    }

    public void ChangColorStep(int stepChange)
    {
        colorSteps -= stepChange;
        if(colorSteps <= 0) colorSteps = 0;
        fullScreenMat.SetFloat(ColorStepID,colorSteps);
    }

    
}

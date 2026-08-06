using UnityEngine;

public class FilterTool_Pixel : FilterTool_Abs
{
    private static readonly int PixelSizeID = Shader.PropertyToID("_PixelSize");
    [SerializeField] private float pixelSize;

    public override void StartUpActivity()
    {
        rendererFeature.SetActive(activeState);
        fullScreenMat.SetFloat(PixelSizeID,pixelSize);
    }
}

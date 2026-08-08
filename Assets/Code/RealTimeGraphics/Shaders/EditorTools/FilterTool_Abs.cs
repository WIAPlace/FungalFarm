using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FilterTool_Abs : MonoBehaviour
{
    [SerializeField] protected ScriptableRendererFeature rendererFeature;
    [SerializeField] protected Material fullScreenMat;
    [SerializeField] protected bool activeState;
    [HideInInspector] public bool activeInEditor;

    public void ChangeActive(bool change)
    {   // should only be accessed for in editor stuff
        rendererFeature.SetActive(change);
    }

    public virtual void StartUpActivity()
    {
        rendererFeature.SetActive(activeState);
    }

    public void TurnOffInEditor()
    {
        if (!activeInEditor)
        {
            rendererFeature.SetActive(false);
        }
    }


    //////////////////////////////////////// Start up and Validate
    protected void OnValidate()
    {
        if (!activeState || activeInEditor)
        {
            StartUpActivity();
        }
    }

    protected void OnEnable()
    {
        if (activeState)
        {
            StartUpActivity();
        }
    }

    protected void OnDisable()
    {
        rendererFeature.SetActive(false);
    }
}

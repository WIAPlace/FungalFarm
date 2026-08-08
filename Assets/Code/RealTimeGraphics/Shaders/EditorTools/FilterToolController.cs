using UnityEngine;

// sets filters as active in inspector or inactive
public class FilterToolController : MonoBehaviour
{
    [SerializeField] private bool activeInEditMode;
    [SerializeField] private FilterTool_Abs[] filterTools;

    void OnValidate()
    {
        foreach(FilterTool_Abs tool in filterTools)
        {
            tool.activeInEditor = activeInEditMode; 
            tool.StartUpActivity(); 
            tool.TurnOffInEditor(); 
        }
    }
}

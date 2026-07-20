using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WindowManager))]
public class WindowManagerEditor : Editor 
{

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        WindowManager windowDrag = (WindowManager)target;

        if (GUILayout.Button("Reset Window"))
        {
            windowDrag.SetPosition();
        }
        
    }
}

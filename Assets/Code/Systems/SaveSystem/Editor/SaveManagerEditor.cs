using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
        SaveManager saveLoadSystem = (SaveManager) target;
        //string gameName = saveLoadSystem.saveData.Name;
            
        DrawDefaultInspector();
        
        /*
        if (GUILayout.Button("New Game")) {
            saveLoadSystem.NewGame();
        }

        */
        if (GUILayout.Button("Save Game")) {
            saveLoadSystem.SaveGameData();
        }
        /*
        if (GUILayout.Button("Load Game")) {
            saveLoadSystem.LoadGame(gameName);
        }

        if (GUILayout.Button("Delete Game")) {
            saveLoadSystem.DeleteGame(gameName);
        }
        */
    }
}

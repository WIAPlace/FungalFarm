using UnityEngine;
using UnityEngine.UIElements;

public class MenuOptions : MonoBehaviour
{
    public UIDocument doc;
    private VisualElement root;
    private VisualElement myUIRoot;
    private Button but;
    void Awake()
    {
        doc = GetComponent<UIDocument>();

        root = doc.rootVisualElement;
        VisualElement myUIRoot = root.Q<VisualElement>("my-panel-root");

        but = doc.rootVisualElement.Q("Exit") as Button;
        but.RegisterCallback<ClickEvent>(OnExitClick);
    }

    private void OnExitClick(ClickEvent evt)
    {
        Application.Quit();
    }

    public void ToggleClicks(bool condition)
    {
        if(condition){
            root.SetEnabled(true);
            root.style.display = DisplayStyle.Flex;
        }
        else {
            root.SetEnabled(false);
            root.style.display = DisplayStyle.None;
        }
    }
}

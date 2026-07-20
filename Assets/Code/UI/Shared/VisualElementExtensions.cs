using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    /// <summary>
    /// Finds a named child element, transfers all stylesheets from this
    /// container to it, and returns it ready for reparenting.
    /// Typically called on a TemplateContainer returned by Instantiate().
    /// </summary>
    public static VisualElement ExtractRoot(this VisualElement container, string rootName)
    {
        var root = container.Q<VisualElement>(rootName);

        if (root == null)
        {
            Debug.LogError($"ExtractRoot: element '{rootName}' not found in container.");
            return null;
        }

        for (int i = 0; i < container.styleSheets.count; i++)
        {
            root.styleSheets.Add(container.styleSheets[i]);
        }

        return root;
    }


    // Aquired From Seprate Sourse
    public static VisualElement CreateChild(this VisualElement parent, params string[] classes) {
        var child = new VisualElement();
        child.AddClass(classes).AddTo(parent);
        return child;
    }
    
    public static T CreateChild<T>(this VisualElement parent, params string[] classes) where T : VisualElement, new() {
        var child = new T();
        child.AddClass(classes).AddTo(parent);
        return child;
    }

    public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement {
        parent.Add(child);
        return child;
    }

    public static T AddClass<T>(this T visualElement, params string[] classes) where T : VisualElement {
        foreach (string cls in classes) {
            if (!string.IsNullOrEmpty(cls)) {
                visualElement.AddToClassList(cls);
            }
        }
        return visualElement;
    }
    
    public static T WithManipulator<T>(this T visualElement, IManipulator manipulator) where T : VisualElement {
        visualElement.AddManipulator(manipulator);
        return visualElement;
    }
}
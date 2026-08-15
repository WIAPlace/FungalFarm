using UnityEngine;

public class ViewHostInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private HostView view;

    public void BeginInteract(out float waitTime, out float staminaDrain, ref InteractionType type)
    {
        waitTime = 0f;
        staminaDrain = 0f;
        if(type == InteractionType.Basic) type = InteractionType.Basic;
    }

    public void EndInteract(float currentWait, ref InteractionType type)
    {
        if(type!=InteractionType.Basic)return;
        GameManager.Instance.CheckConditionBar(view.details.condition,299, view.hostName);
    }
}

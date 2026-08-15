using UnityEngine;

public class OpenShop : MonoBehaviour, IInteractable
{
    public void BeginInteract(out float waitTime, out float staminaDrain, ref InteractionType type)
    {
        waitTime = 0;
        staminaDrain = 0;
        type = InteractionType.Basic;
    }

    public void EndInteract(float currentWait, ref InteractionType type)
    {
        GameManager.Instance.ToggleShop(true);
    }

}

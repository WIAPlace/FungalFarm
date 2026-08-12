using UnityEngine;

public class OpenInventory : MonoBehaviour, IInteractable
{
    public int inventoryIndex;

    public void BeginInteract(out float waitTime, out float staminaDrain,ref InteractionType type)
    {
        staminaDrain = 0;
        waitTime=.1f;
        type = InteractionType.Basic;

    }

    public void EndInteract(float currentWait, ref InteractionType type)
    {
        GameManager.Instance.OpenSpecificInventory(inventoryIndex);
    }

    
}

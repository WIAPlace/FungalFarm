using UnityEngine;

public class OpenInventory : MonoBehaviour, IInteractable
{
    public int inventoryIndex;

    public void BeginInteract(out float waitTime, out float staminaDrain)
    {
        staminaDrain = 0;
        waitTime=.1f;

    }

    public void EndInteract(float currentWait)
    {
        GameManager.Instance.OpenSpecificInventory(inventoryIndex);
    }

    
}

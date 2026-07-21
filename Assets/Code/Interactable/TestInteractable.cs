using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public float timeToWait;
    public ItemsDataBase idb;
    public Item item;

    public void BeginInteract(out float waitTime)
    {
        //throw new System.NotImplementedException();
        waitTime = timeToWait;
    }

    public void EndInteract(float currentWait)
    {
        if(currentWait < timeToWait) return; 
        Debug.Log("Interact Completed");
        //throw new System.NotImplementedException();
        if(idb.items.TryRemove(default(Item))) Debug.Log("Deleted Item");
        else Debug.Log("Failed To Delete Item");
        if(idb.items.TryAdd(item)) Debug.Log("Obtained Item");
        else Debug.Log("Failed To Get item");
        Destroy(gameObject);
    }
}

using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    public float timeToWait;

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
    }
}

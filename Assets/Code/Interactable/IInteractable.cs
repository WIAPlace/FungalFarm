using UnityEngine;

public interface IInteractable
{
    public void BeginInteract(out float waitTime, out float staminaDrain); // gives how long the process should take (Default at least)
    public void EndInteract(float currentWait); // On Completion 
}

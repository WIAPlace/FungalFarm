using UnityEngine;

public interface IInteractable
{
    public void BeginInteract(out float waitTime, out float staminaDrain, ref InteractionType type); // gives how long the process should take (Default at least)
    public void EndInteract(float currentWait, ref InteractionType type); // On Completion 
}

public enum InteractionType
{
    Basic,
    Water,
    Milk,
    Trowel,
    Spore
}

using System;
using UnityEditor.Search;
using UnityEngine;

public class SporeableSpotInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    [SerializeField] private float interactTime; 
    [SerializeField] private HostView host;
    public int indexLocation;

    public void BeginInteract(out float waitTime)
    {
        waitTime = interactTime;
    }

    public void EndInteract(float currentWait)
    {
        if(currentWait<interactTime) return;
        // take away stamina from player.
    }
}

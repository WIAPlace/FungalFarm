using System;
using UnityEditor.Search;
using UnityEngine;

public class SporeableSpotInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactTime; 
    //[SerializeField] private Host host;
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

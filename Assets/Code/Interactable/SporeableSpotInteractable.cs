using System;
using UnityEditor.Search;
using UnityEngine;

public class SporeableSpotInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    
    [field : SerializeField] public float baseInteractTime; 
    [HideInInspector] public float interactTime;
    public float staminaDrainAmt;
    [SerializeField] private HostView host;
    public int indexLocation;

    public SporeSpotState currentState;

    // add bool that is for if it can be interacted with while empty

    public void BeginInteract(out float waitTime,out float staminaDrain)
    {
        waitTime = interactTime;
        staminaDrain = staminaDrainAmt;
    }

    public void EndInteract(float currentWait)
    {
        if(currentWait<interactTime) return;
        // take away stamina from player.
        if(currentState == SporeSpotState.Sporeable) {
            host.AddMushroomToHost(indexLocation);
        }
        else if(currentState == SporeSpotState.Harvestable)
        {
            host.RemoveMushroom(indexLocation); // this will be changed to harvest mushroom, for now though while we don't know what state the player is in it will be this
        }
    }

    public void SetState(SporeSpotState state)
    {
        //Debug.Log(state);
        currentState = state;
        if(currentState == SporeSpotState.Growing)
        {
            gameObject.layer = 0;
        }
        else gameObject.layer = 7;
    }
}

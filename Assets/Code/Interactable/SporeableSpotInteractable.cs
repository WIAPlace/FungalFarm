using System;

using UnityEngine;
using UnityEngine.AI;

public class SporeableSpotInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    
    [field : SerializeField] public float baseInteractTime; 
    [HideInInspector] public float interactTime;
    public float staminaDrainAmt;
    [SerializeField] private HostView host;
    public int indexLocation;

    public SporeSpotState currentState;

    
    void OnDestroy()
    {
        host = null;
    }
    // add bool that is for if it can be interacted with while empty

    public void BeginInteract(out float waitTime,out float staminaDrain,ref InteractionType type)
    {
        waitTime = interactTime;
        staminaDrain = staminaDrainAmt;
        
        if(currentState == SporeSpotState.Sporeable)
        {
            type = InteractionType.Spore;
        }
        else if(type == InteractionType.Basic)
        {
            waitTime = 0.01f;
            staminaDrain = 0;
            type = InteractionType.Basic;
            return;
        }
        else if(type == InteractionType.Milk)
        {
            type = InteractionType.Milk;
        }
        else if(type == InteractionType.Water)
        {
            type = InteractionType.Water;
        }
        else if(currentState == SporeSpotState.Harvestable)
        {
            // harvest shroom
            type = InteractionType.Trowel;
        }
        else if(currentState == SporeSpotState.Growing && type == InteractionType.Trowel)
        {   // remove shroom;
            type = InteractionType.Trowel;
        }
        else type = InteractionType.Basic;
    }

    public void EndInteract(float currentWait, ref InteractionType type)
    {
        //if(currentWait<interactTime) return;
        // take away stamina from player.
        if(type == InteractionType.Basic && currentWait<interactTime)
        {
            if(host.details.mushrooms[indexLocation]==null) return;
            MushroomInstance shroom = host.details.mushrooms[indexLocation];
            if(shroom.details==null) return;

            int currentStage = shroom.currentStage;
            int maxStage = shroom.details.MaxStageAmt;
            float conEffect = shroom.details.conditionEffect;

            GameManager.Instance.CheckConditionBar(currentStage,maxStage,shroom.details.Name,conEffect);

            return;
        }
        
        
        if(currentWait<interactTime) return;
        
        if(currentState == SporeSpotState.Sporeable) {
            host.AddMushroomToHost(indexLocation);
        }
        else if(type == InteractionType.Milk)
        {
            SetPrioirtyBonus(10);
            Instantiate(HostManager.Instance.milkParticle, this.transform.position,this.transform.rotation, this.transform);
        }
        else if(type == InteractionType.Water)
        {
            SetWatered();
        }
        else if(currentState == SporeSpotState.Harvestable)
        {
            if(GetShroom()) host.HarvestMushroom(indexLocation); // this will be changed to harvest mushroom, for now though while we don't know what state the player is in it will be this
            else Debug.Log("Cant Harvest");
        }
        else if(currentState == SporeSpotState.Growing && type == InteractionType.Trowel)
        {
            host.RemoveMushroom(indexLocation);
        }
    }

    public void SetState(SporeSpotState state)
    {
        //Debug.Log(state);
        currentState = state;
        
        if(host.isCreature&& currentState != SporeSpotState.Harvestable)
        {
            gameObject.layer = 0;
        }
        else if(host.isCreature)gameObject.layer = 7;
        
    }

    public void SetPrioirtyBonus(int priorityBonus)
    {
        host.SetMushroomPriority(indexLocation,priorityBonus);
    }
    public void SetWatered()
    {
        host.SetShroomAsWatered(indexLocation);
    }
    public bool GetShroom()
    {
        return host.GetShroom(indexLocation);
    }
}

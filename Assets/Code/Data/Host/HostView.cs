using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum Condition
{
    unusable = -1,
    dead = 99,
    rotting = 199,
    healthy = 299
}

public enum SporeSpotState
{
    Sporeable,
    Growing,
    Harvestable
}

// attach to  the game object host. will act as the interface between the player and the controller.
// also where starting amounts will be declared
public class HostView : MonoBehaviour
{
    [field:SerializeField] public SerializableGuid ID = SerializableGuid.NewGuid();
    public int managerIndex;

    public bool isCreature; 
    public CreatureType creatureType = CreatureType.None;

    public Condition startingCondition;
    public SporeableSpotInteractable[] sporeSpots;

    public SporeableMushrooms_SO sporeableMushrooms; // used for what mushrooms are able to be planted here 

    [SerializeReference] public HostDetails details;

    public void Initialize()
    {
        for(int i = 0; i<sporeSpots.Length; i++)
        {
            sporeSpots[i].SetState(SporeSpotState.Sporeable);
            sporeSpots[i].indexLocation = i;
            ChangeInteractTime(i,-1); // set interact time to base amt;
        }
    }

    public void RemoveMushroom(int sporeIndex) 
    { // event to set off that a mushroom has been removed at cetain index
        if(sporeIndex >= sporeSpots.Length) return; // if out of range dont sent out the call

        HostManager.Instance.MushroomRemoved(managerIndex,sporeIndex);
    }

    public void HarvestMushroom(int sporeIndex)
    {
        if(sporeIndex >= sporeSpots.Length) return; // if out of range dont sent out the call
        HostManager.Instance.MushroomHarvested(managerIndex,sporeIndex);
    }


    public void ChangeInteractTime(int index, float waitTime)
    {
        if(index>=sporeSpots.Length) return;
        if(waitTime < 0) sporeSpots[index].interactTime = sporeSpots[index].baseInteractTime;
        else sporeSpots[index].interactTime = waitTime;
    }

    public void ChangeSporeSpotModel(int index,GameObject mushStatePrefab)
    {
        // change mushroom of model of sporespot 
        RemoveSporeSpotModel(index); // clear out any chilcren
        Transform sporeSpotTransform = sporeSpots[index].gameObject.transform;
        
        GameObject newShroom = Instantiate(mushStatePrefab,sporeSpotTransform.position,sporeSpotTransform.rotation);

        newShroom.transform.SetParent(sporeSpotTransform,true);
    }

    public void SetSporeSpotInteractivity(int index, SporeSpotState state)
    {
        //Debug.Log("setter called");
        if(index >= sporeSpots.Length) return;

        //Debug.Log("null Catcher passed");
        sporeSpots[index].SetState(state);
    }

    public void RemoveSporeSpotModel(int index)
    {
        Transform sporeSpotTransform = sporeSpots[index].gameObject.transform;
        for (int i = sporeSpotTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(sporeSpotTransform.GetChild(i).gameObject);
        }
    }

    public void AddMushroomToHost(int sporeSpotIndex) // called from the spore spot interactable
    {
        HostManager.Instance.NewMushroomAtSporeSpot(managerIndex,sporeSpotIndex);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;



public enum Condition
{
    unusable = -1,
    dead = 99,
    rotting = 199,
    healthy = 299
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


    public void RemoveMushroom(int sporeIndex) 
    { // event to set off that a mushroom has been removed at cetain index
        if(sporeIndex >= sporeSpots.Length) return; // if out of range dont sent out the call

        HostManager.Instance.MushroomRemoved(managerIndex,sporeIndex);
    }


    public void ChangeSporeSpotModel(int index,GameObject mushStatePrefab)
    {
        // change mushroom of model of sporespot 
    }
    public void RemoveSporeSpotModel(int index)
    {
        
    }
}

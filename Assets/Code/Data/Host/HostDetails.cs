using System;
using System.Diagnostics;
using UnityEngine;

// a class to store data about the host.
[Serializable]
public class HostDetails 
{
    [field:SerializeField] public SerializableGuid ID = SerializableGuid.NewGuid();
    public SerializableGuid viewID; // ID for the Game object this is related to
    public int index;
    
    [Header("Creature Hosts")]
    public bool isCreature;
    [field : SerializeReference] public CreatureInstance creature;

    [Header("Condition And Effects")]
    public bool isInfested;

    //-1 is unusable
    //0-99 is dead
    //100-199 is decaying
    //200-299 is healthy
    public int condition; 

    [Header("Spore Spots")]
    public int sporeSpotAmt;
    public MushroomInstance[] mushrooms;
    public SerializableGuid[] mushroomsIDs;
    //public SporeableMushrooms_SO sporeAble;
    
}

using System;
using UnityEngine;

// a class to store data about the host.
[Serializable]
public class HostDetails 
{
    [field:SerializeField] public SerializableGuid ID = SerializableGuid.NewGuid();
    public SerializableGuid viewID; // ID for the Game object this is related to
    public int index;

    //-1 is unusable
    //0-99 is dead
    //100-199 is decaying
    //200-299 is healthy
    public int condition; 

    public int sporeSpotAmt;
    public MushroomInstance[] mushrooms;
    public SerializableGuid[] mushroomsIDs;
    
}

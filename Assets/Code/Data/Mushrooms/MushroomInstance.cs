using System;
using UnityEngine;

[Serializable]
public class MushroomInstance
{
    [field: SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    public SerializableGuid dataId => details.Id; 
    public SerializableGuid hostID => host.ID;
    public MushroomDetails details = null;
    public HostDetails host;

    public int sporeIndex;

    public int currentStage = 0;
    public int progressToNextStage = 0;

    // has this fella been cared for recently, like moist n shit
    public bool nurtured = false; 

    // if the enviroment is sutible for it, is the tree it's on dying? and is that good.
    public bool enviromentBonus = false; 

    public bool harvestable = false;
    
    public MushroomInstance(MushroomDetails details, HostDetails host)
    {
        this.details = details;
        this.host = host;
    }

    
}   

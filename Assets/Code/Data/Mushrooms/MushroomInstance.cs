using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class MushroomInstance : IOnTime
{
    [field: SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    public SerializableGuid dataId => details.Id; 
    public MushroomDetails details;

    public int currentStage = 0;

    // has this fella been cared for recently, like moist n shit
    public bool nurtured = false; 

    // if the enviroment is sutible for it, is the tree it's on dying? and is that good.
    public bool enviromentBonus = false; 
    
    public MushroomInstance(MushroomDetails details)
    {
        this.details = details;
    }

    public void ProgressTimeState(int stages)
    {
        
    }
}   

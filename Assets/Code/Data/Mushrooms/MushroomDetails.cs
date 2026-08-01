using System;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMushroomDetails", menuName = "WorldItems/MushroomDetails")]
[Serializable]
public class MushroomDetails : ScriptableObject
{
    [field:SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    public string Name;
    public int MaxStageAmt; // amount of stages this fungi has.
    public int HarvestableStage; // anything equal or greater than this will be harvestable. likely just max stage amt
    public int StageLength; // how much progress is needed between each stage.
    public float BaseHarvestTime;
    public GameObject[] StagePrefabs; 
    public int conditionEffect = 0; // the ammount of condition that is applied to the host per time change. neg or pos.
    // public traits[] traits // not yet figured out how exactly i wanna go about this yet.

    
    public MushroomInstance Create(HostDetails host)
    {
        return new(this, host);
    }
    
}

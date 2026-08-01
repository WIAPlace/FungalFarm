using UnityEngine;


[CreateAssetMenu(fileName = "NewMushroomDetails", menuName = "WorldItems/MushroomDetails")]
public class MushroomDetails : ScriptableObject
{
    [field:SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    public string Name;
    public int MaxStageAmt; // amount of stages this fungi has.
    public int StageLength; // how much progress is needed between each stage.
    public float BaseHarvestTime;
    public GameObject[] StagePrefabs; 
    public int conditionEffect; // the ammount of condition that is applied to the host per time change. neg or pos.
    // public traits[] traits // not yet figured out how exactly i wanna go about this yet.

    /*
    public MushroomInstance Create(Host host)
    {
        return new(this, host);
    }
    */
}

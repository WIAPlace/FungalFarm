using UnityEngine;


[CreateAssetMenu(fileName = "NewMushroomDetails", menuName = "WorldItems/MushroomDetails")]
public class MushroomDetails : ScriptableObject
{
    [field:SerializeField] public SerializableGuid Id = SerializableGuid.NewGuid();
    public string Name;
    public int MaxStageAmt;
    public float BaseHarvestTime;
    public GameObject[] StagePrefabs; 
    // public traits[] traits // not yet figured out how exactly i wanna go about this yet.

    /*
    public MushroomInstance Creat()
    {
        
    }
    */
}

using UnityEngine;

[CreateAssetMenu(fileName = "FungiButtonInfo", menuName = "WorldItems/FungiButtonInfo")]
public class FungiButtonInfo : ScriptableObject
{
    public MushroomDetails shroom;
    public Sprite icon;
    public Sprite MajorImage;
    public string description;
    public string conditionEffect;
    public string stages;
    public string growLocations;
    public string shroomName;
}

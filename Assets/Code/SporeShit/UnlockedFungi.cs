using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnlocks", menuName = "WorldItems/UnlockedFungi")]
public class UnlockedFungi : ScriptableObject
{
    public List<SerializableGuid> unlocks = new List<SerializableGuid>();

    public bool CheckIfUnlocked(MushroomDetails dets)
    {
        if(unlocks==null) return false;
        foreach(SerializableGuid mushID in unlocks)
        {
            if(mushID == dets.Id) return true;
        }
        return false;
    }

    public void UnlockFungi(MushroomDetails dets)
    {
        if (!CheckIfUnlocked(dets))
        {
            unlocks.Add(dets.Id);
        }
    }

}

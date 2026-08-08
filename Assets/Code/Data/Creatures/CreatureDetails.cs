using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCreatureDetails", menuName = "WorldItems/CreatureDetails")]
[Serializable]
public class CreatureDetails : ScriptableObject
{
    [field:SerializeField] public SerializableGuid ID = SerializableGuid.NewGuid();
    public CreatureType creatureType = CreatureType.None;

    public CreatureInstance Create(HostDetails host)
    {
        return new CreatureInstance(host,this);
    }
}

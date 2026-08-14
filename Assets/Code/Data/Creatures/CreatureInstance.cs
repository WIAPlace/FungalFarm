using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CreatureInstance
{
    [field:SerializeField] public SerializableGuid ID = SerializableGuid.NewGuid();
    public int index;

    [field : SerializeReference] public CreatureDetails data;
    public SerializableGuid dataID => data.ID;
    public int currentHungerStage=0;
    [field : SerializeReference] public HostDetails host;


    public CreatureInstance(HostDetails host, CreatureDetails details)
    {
        data = details;
        this.host = host;
    } 


    public void RemoveSelf()
    {
        
    }
}

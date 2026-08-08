using System;
using UnityEngine;
using System.Collections.Generic;

public class CreatureManager : MonoBehaviour, IOnTime
{
    public static CreatureManager Instance {get;private set;}

    private void Awake()
    {
        // 2. Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // Destroy duplicate if one is already active
            Destroy(gameObject); 
            return;
        }

        // 3. Set the active instance to this object
        Instance = this;

        // Optional: Keep this object alive across scene transitions
        //DontDestroyOnLoad(gameObject);
    }

    public CreatureInstance[] creatures;

    [Tooltip("how many time states need to pass for a creature host to get hungry")]
    public int creatureHungerCycle; // might be changed to a regrence to an outside script having to do with creatures

    // the bool is to let us know if it worked
    public bool TryAddCreatureToForest()  ////////////////////////////////////////////////// Try Add
    {
        for(int i = 0; i < creatures.Length; i++)
        {
            if(creatures[i]!=null || creatures[i].ID==SerializableGuid.Empty) continue;
            
            return true;
        }
        return false;
    }
    
    public void RemoveCreatureFromForest(int index) //////////////////////////////////////// Remove
    {
        if(index >= creatures.Length) return;
        creatures[index].RemoveSelf();
        creatures[index] = null;
    }

    public void ProgressTimeState(int stages)
    {
        for(int i = 0; i<stages;i++){
            HostManager.Instance.RefreshEdibleList();
            List<MushroomInstance> edibleList = HostManager.Instance.edibleList;

            foreach(CreatureInstance creat in creatures)
            {
                if(creat == null || creat.dataID == SerializableGuid.Empty) continue;
                creat.currentHungerStage += 1;
                
                if(creat.currentHungerStage >= creatureHungerCycle)
                {
                    // eat the one of highest priority
                    if(edibleList==null && edibleList[0]==null) continue;

                    EdibleEffect effect = edibleList[0].details.edibleEffect;

                    if(effect == EdibleEffect.Infected)
                    {
                        HostManager.Instance.NewMushroomAtSporeSpot(creat.host.index);
                    }

                    HostManager.Instance.UpdateHostCondition(creat.host, edibleList[0].details.conditionEffect);

                    HostManager.Instance.MushroomRemoved(edibleList[0].sporeIndex,edibleList[0].host.index);
                    edibleList.RemoveAt(0);
                    
                    // look through switch statment to have effects occure
                    // maybe if it gets death a number of times it dies, though it would be nice if it was tougher for certain shrooms
                }
            }
        }
    }

}

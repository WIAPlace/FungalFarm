using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public enum Condition
{
    unusable = -1,
    dead = 99,
    rotting = 199,
    healthy = 299
}

public enum SporeSpotState
{
    Sporeable,
    Growing,
    Harvestable
}

// attach to  the game object host. will act as the interface between the player and the controller.
// also where starting amounts will be declared
public class HostView : MonoBehaviour
{
    [field:SerializeField] public SerializableGuid ID = SerializableGuid.NewGuid();
    public int managerIndex;


    [Header("Starting Info")]
    public bool isCreature; 
    public CreatureType creatureType = CreatureType.None;
    [SerializeField] public CreatureDetails creatureDetails;


    public Condition startingCondition;
    public Condition destroyCondition;
   
    public HostView onDeathSpawn;
    [HideInInspector] public bool destroyOnInvis=false;

    [Header("Spore Information")]
    public SporeableSpotInteractable[] sporeSpots;

    public SporeableMushrooms_SO sporeableMushrooms; // used for what mushrooms are able to be planted here 

    [Header("Objects Active At Each State"),Tooltip("Anything That is placed in one of these will be made inactive if place in another.")]
    public GameObject[] UnusableState;
    public GameObject[] DeadState;
    public GameObject[] RottingState;
    public GameObject[] HealthyState;
    public GameObject[][] VisualStateHolders;



    [Header("Host Details Ref")]
    
    public Condition currentCon;
    [SerializeReference] public HostDetails details;

    void OnDestroy()
    {
        creatureDetails = null;
        sporeSpots=null;
        onDeathSpawn = null;
        sporeableMushrooms =null;
        UnusableState = null;
        DeadState = null;
        RottingState = null;
        HealthyState = null;
        VisualStateHolders = null;
        details = null;
    }

    public void Initialize()
    {
        // place all visual states into the visual state holder
        destroyOnInvis=false;
        VisualStateHolders = new GameObject[][]
        {
            UnusableState,DeadState,RottingState,HealthyState
        };

        // set visuals to how they should be
        currentCon=startingCondition;
        ClearAllVisuals();
        MakeVisible(ConditionToInt(currentCon));

        for(int i = 0; i<sporeSpots.Length; i++)
        {
            sporeSpots[i].gameObject.layer = 7;
            sporeSpots[i].SetState(SporeSpotState.Sporeable);
            sporeSpots[i].indexLocation = i;
            ChangeInteractTime(i,-1); // set interact time to base amt;
        }
    }

    public void RemoveMushroom(int sporeIndex) 
    { // event to set off that a mushroom has been removed at cetain index
        if(sporeIndex >= sporeSpots.Length) return; // if out of range dont sent out the call

        HostManager.Instance.MushroomRemoved(managerIndex,sporeIndex);
    }

    public void HarvestMushroom(int sporeIndex)
    {
        if(sporeIndex >= sporeSpots.Length) return; // if out of range dont sent out the call
        HostManager.Instance.MushroomHarvested(managerIndex,sporeIndex);
    }


    public void ChangeInteractTime(int index, float waitTime)
    {
        if(index>=sporeSpots.Length) return;
        if(waitTime < 0) sporeSpots[index].interactTime = sporeSpots[index].baseInteractTime;
        else sporeSpots[index].interactTime = waitTime;
    }

    public void ChangeSporeSpotModel(int index,GameObject mushStatePrefab)
    {
        // change mushroom of model of sporespot 
        RemoveSporeSpotModel(index); // clear out any chilcren
        Transform sporeSpotTransform = sporeSpots[index].gameObject.transform;
        
        GameObject newShroom = Instantiate(mushStatePrefab,sporeSpotTransform.position,sporeSpotTransform.rotation);

        newShroom.transform.SetParent(sporeSpotTransform,true);
    }

    public void SetSporeSpotInteractivity(int index, SporeSpotState state)
    {
        //Debug.Log("setter called");
        if(index >= sporeSpots.Length) return;

        //Debug.Log("null Catcher passed");
        sporeSpots[index].SetState(state);
    }

    public void RemoveSporeSpotModel(int index)
    {
        Transform sporeSpotTransform = sporeSpots[index].gameObject.transform;
        for (int i = sporeSpotTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(sporeSpotTransform.GetChild(i).gameObject);
        }
    }

    public void AddMushroomToHost(int sporeSpotIndex) // called from the spore spot interactable
    {
        HostManager.Instance.NewMushroomAtSporeSpot(managerIndex,sporeSpotIndex);
    }

    //////////////////////////////////////////////////////////////////////////////////////////// Visual Condition States
    public void OnConditionChange(int con)
    {
        Condition newCon = IntToCondition(con);
        if(newCon == currentCon) return;

        currentCon = newCon;
        ClearAllVisuals();
        MakeVisible(ConditionToInt(newCon));

        if(currentCon == destroyCondition)
        {
            Debug.Log("Dead");
            foreach(SporeableSpotInteractable spore in sporeSpots)
            {
                spore.SetState(SporeSpotState.Growing);
            }
            if(onDeathSpawn != null)
            {
                HostManager.Instance.AddDeadViewToArray(onDeathSpawn);
            }
            HostManager.Instance.RemoveHost(managerIndex);
        }
    }

    public void ClearAllVisuals()
    {
        for(int i = 0; i < VisualStateHolders.Length; i++)
        {
            foreach(GameObject visual in VisualStateHolders[i])
            {
                if(visual.activeSelf) visual.SetActive(false);
            }
        }
    }
    
    public void MakeVisible(int state)
    {
        if(state>VisualStateHolders.Length) return;

        foreach(GameObject visual in VisualStateHolders[state])
        {
            if(!visual.activeSelf) visual.SetActive(true);
        }
    }

    public Condition IntToCondition(int con)
    {
        Condition newCon;
        if(con < 0) // unusable
        {
            newCon = Condition.unusable;
        }
        else if (con < 100)
        {
            newCon = Condition.dead;
        }
        else if (con < 200)
        {
            newCon = Condition.rotting;
        }
        else
        {
            newCon = Condition.healthy;
        }
        return newCon;
    }
    public int ConditionToInt(Condition con)
    {
        var returnInt = con switch
        {
            Condition.unusable => 0,
            Condition.dead => 1,
            Condition.rotting => 2,
            Condition.healthy => 3,
            _ => 0,
        };
        return returnInt;
    }


    //////////////////////////////////////////////////////////////////////////// Priority
    public void SetMushroomPriority(int mushIndex, int prioirityBonus)
    {
        HostManager.Instance.SetMushroomPriority(managerIndex,mushIndex,prioirityBonus);
    }

    public void OnChildBecameInvisible()
    {
        Debug.Log("Invis");
        if (destroyOnInvis)
        {
            Destroy(gameObject);
        }
    }

    public void SetShroomAsWatered(int shroomIndex)
    {
        HostManager.Instance.WaterShroom(managerIndex,shroomIndex);
    }

    public bool GetShroom(int shroomIndex)
    {
        return HostManager.Instance.ShoomHarvested(managerIndex,shroomIndex);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

// where all of the logic is held.
public class HostManager : MonoBehaviour, IOnTime
{
    public static HostManager Instance {get;private set;}

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

    // slot order should be as follows, 
    public HostDetails[] hosts; // will be added during play
    [field:SerializeField,Tooltip("Set Up in Inspector")]
        public HostView[] views; // set up in inspector

    public float chanceToSpore;
    [Tooltip("how many time states need to pass for a creature host to get hungry")]
    public int creatureHungerCycle; // might be changed to a regrence to an outside script having to do with creatures

    public List<MushroomInstance> edibleList; // will be used to sift through what is edible

    private void Start()
    {
        InitializeDetailsToViews();
    }

    private void OnDestroy()
    {
        
    }

    public void InitializeDetailsToViews() // occurs at game start.
    {
        hosts = new HostDetails[views.Length];

        for(int i = 0; i < views.Length; i++)
        {
            SetHostAtIndexI(i); // seperated so that we can set host during play as well.
        }
    }
    
    // initialize data from a view at a certain point in the host array
    public void SetHostAtIndexI(int i)
    {
        views[i].managerIndex = i; // set index refrence
        HostDetails newDetails = new(); // create new details
        newDetails.viewID = views[i].ID; // add viewID refrence
        newDetails.index = i; // set index refrence
        newDetails.isCreature = views[i].isCreature; // set its status as if its creature
        newDetails.creatureType = views[i].creatureType;

        // spawn with a random hunger amt, only applies to creature hosts.
        newDetails.currentHungerStage = UnityEngine.Random.Range(0,creatureHungerCycle); 

        // will need to set up what their starting condition is.
        newDetails.condition = (int)views[i].startingCondition; // set condition as starting condition, will want to load stuff after this.

        newDetails.sporeSpotAmt = views[i].sporeSpots.Length; // add slots equal to the amount of mushrooms length.
        newDetails.mushrooms = new MushroomInstance[newDetails.sporeSpotAmt]; // set up mushroom slots
        newDetails.mushroomsIDs = new SerializableGuid[newDetails.sporeSpotAmt]; // set up their IDs

        for(int b = 0; b < newDetails.sporeSpotAmt; b++) // set up slot ids
        {
            newDetails.mushroomsIDs[b] = views[i].sporeSpots[b].Id; // set slot id to the id of the views slot.
            newDetails.mushrooms[b].Id = newDetails.mushroomsIDs[b]; // set the slot as this id.
        }

        // Might want to add the details to the game object as a ref just so we can see it while in editor

        hosts[i] = newDetails; // add to host array.
    }

    public void ProgressTimeState(int stages) // as time ticks past. 
    {
        if(hosts==null || hosts.Length<1) return; // catch for if not set up corectly

        foreach(HostDetails host in hosts)       /// Update Hosts
        {// chose foreach because its easier to type and we have a ref to index in host anyways
            
            if(host==null || host.condition < 0) return; // skip if non existant or unusable
            // might want to add a effect that will clear if this is < 0; or do that in the body. 
            
            for(int i = 0; i < stages; i++){
                // EFFECTS On CONDITION
                int conditionEffect = 0; // will be added to host's condition

                foreach(MushroomInstance mush in host.mushrooms)       ///////// Mushrooms 
                {
                    if(mush == null || mush.details == null) continue; // skip if this slot is empty

                    // add mushroom's condition effect to the tree's condition
                    conditionEffect += mush.details.conditionEffect; 

                    if(mush.currentStage < mush.details.MaxStageAmt) // update stage of Mushroom
                    {
                        int newStage = UpdateMushroomStage(mush);
                        if (newStage > mush.currentStage)
                        {
                            mush.currentStage = newStage;
                            if(mush.currentStage >= mush.details.HarvestableStage) mush.harvestable = true;

                            // effect the view in some way. // like updating the prefab
                            views[host.index].ChangeSporeSpotModel(mush.sporeIndex, mush.details.StagePrefabs[newStage]);
                            
                        }
                    }

                    // update spore spot in some way to reflect any changes here.
                }

                int maxAmt = 299; // at certain stages the host cant get better.
                if(host.condition < 100) // if host is already dead
                { 
                    maxAmt = 99;
                }

                int newCondition = Mathf.Clamp( host.condition + conditionEffect ,-1, maxAmt );
                
                if(newCondition < 0)
                {
                    // set as unusable, for the view set it in some visual way
                }

                if (host.isCreature)
                {
                    host.currentHungerStage += 1;
                    if(host.currentHungerStage <= creatureHungerCycle && newCondition < host.condition)
                    {
                        host.currentHungerStage = 0;// reset cycle stage.
                    }
                    // if condition goes down try to eat a food and add that to condition
                }

                host.condition = newCondition;
            }
        }
    }


    public int UpdateMushroomStage(MushroomInstance mush)
    {
        int newStage=mush.currentStage;
        int stageAdditionAmt = 1; // if nothing else it will add 1 

        if(mush.nurtured) stageAdditionAmt += 2;
        if(mush.host.isInfested) stageAdditionAmt /= 2;
        
        mush.progressToNextStage += stageAdditionAmt;

        // updates stage and progress. 
        // its a while so in the off chance that it is has gone up a greater than 1 stage it will be correct 
        while (mush.progressToNextStage >= mush.details.StageLength)
        {
            newStage++;
            mush.progressToNextStage -= mush.details.StageLength;
        }

        return newStage;
    }


    // add spores of a mushroom to a new spot
    // should have already checked if the spot is empty.
    public void NewMushroomAtSporeSpot(int hostIndex, int sporeSpotIndex, MushroomDetails details) ////////////////////////// Mushroom Added
    {
        // if host index is too long, or host index doesnt exist, or host condition is less than 0 or spore spot index is greater than sporespots avalible 
        if(hostIndex >= hosts.Length || hosts[hostIndex] == null || 
            hosts[hostIndex].condition < 0 || sporeSpotIndex >= hosts[hostIndex].sporeSpotAmt) {
                Debug.Log("Sporeing at this spot is not accepted");
                return;
            }

        MushroomInstance newMush = details.Create(hosts[hostIndex]); // creates a new mushroom with details based off of the host     
        newMush.Id = hosts[hostIndex].mushroomsIDs[sporeSpotIndex]; // set the mush id to the slot id.
        hosts[hostIndex].mushrooms[sporeSpotIndex] = newMush; // add mushroom to slot

        // do some visual shit with the view
        views[hostIndex].ChangeSporeSpotModel(sporeSpotIndex, hosts[hostIndex].mushrooms[sporeSpotIndex].details.StagePrefabs[0]);
    }  

    public void MushroomRemoved(int hostIndex, int sporeIndex) ///////////////////////////////////////////////// Mushroom Removed
    {
        if(hostIndex>=hosts.Length || hosts[hostIndex]==null) return;

        hosts[hostIndex].mushrooms[sporeIndex] = null;
        // Make sure that when this is set to null the dataID is set to 0000000000 / empty
        // or create a mushroom details that is just empty that can be refrenced as such

        //Update view's spore spots.
        views[hostIndex].RemoveSporeSpotModel(sporeIndex);
    }

    public void MushroomHarvested(int hostIndex, int sporeIndex) ///////////////////////////////////////////////// Mushroom Harvested
    {
        if(hostIndex>=hosts.Length || hosts[hostIndex]==null) return;

        hosts[hostIndex].mushrooms[sporeIndex].harvestable = false;
        hosts[hostIndex].mushrooms[sporeIndex].currentStage = 1;

        views[hostIndex].ChangeSporeSpotModel(sporeIndex, hosts[hostIndex].mushrooms[sporeIndex].details.StagePrefabs[1]);
    }


    // Add A function that will add a new host view to the array, and then call setHostAtIndexI at that array position in hosts.
    // Eithier wilol be called from here or more likeley outside of here. if in here we'll need to instantiate a prefab in the world at a postion. 
}

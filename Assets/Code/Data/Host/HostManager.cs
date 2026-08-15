using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

// where all of the logic is held.
[DefaultExecutionOrder(-100)]
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
    public int deadHostsIndexDivider;
    public int creatureHostsIndexDivider;
    public ItemsDataBase invData;
    [HideInInspector]public List<HostView> InitialHostViews = new List<HostView>(); 

    public GameObject[] deadHostSlotPositions;
    
    bool startUp = true;

    //public LayerMask interactMask; // used for refrence in other stuff. set here so it doesnt need to be individualy set

    public List<MushroomInstance> edibleList = new(); // will be used to sift through what is edible

    private void Start()
    {
        //startUp=true;
        TimeManager.Instance.ManageTimer(this); // adds this to managed timers.

        NewGame();
    }

    private void OnDestroy()
    {
        
    }

    public void AddIfNotAlready(HostView view)
    {
        if(InitialHostViews!=null && !InitialHostViews.Contains(view))
        {
            InitialHostViews.Add(view);
        }
    }

    public void NewGame()
    {
        startUp = true;
        InitializeDetailsToViews();
    }

    public void InitializeDetailsToViews() // occurs at game start.
    {
        deadHostsIndexDivider += InitialHostViews.Count;
        creatureHostsIndexDivider += InitialHostViews.Count;
        views = new HostView[creatureHostsIndexDivider + 8];

        for(int i = 0; i < InitialHostViews.Count; i++)
        {   // pass it from one to another
            views[i] = InitialHostViews[i];
        }

        hosts = new HostDetails[views.Length];

        for(int i = 0; i < views.Length; i++)
        {
            //SetHostAtIndexI(i); // seperated so that we can set host during play as well.
            if(startUp) SetHostAtIndexI(i);
            else if(SaveManager.Instance != null && SaveManager.Instance.saveData!=null && SaveManager.Instance.saveData.hosts!=null) LoadHostData(i);
            else SetHostAtIndexI(i);
        }
        if(startUp) startUp = false;
    }
    // 
    
    
    // initialize data from a view at a certain point in the host array
    public void SetHostAtIndexI(int i) ///////////////////////////////////////////////////////////////////////////// Initialize
    {
        if(views[i]==null) {
            hosts[i] = null;
            return;
        }

        views[i].managerIndex = i; // set index refrence
        views[i].Initialize(); // initialize in a more controlled fassion than their own start.
        HostDetails newDetails = new(); // create new details
        newDetails.viewID = views[i].ID; // add viewID refrence
        newDetails.index = i; // set index refrence
        newDetails.isCreature = views[i].isCreature; // set its status as if its creature
        newDetails.veiwType = views[i].veiwType;
        

        if(newDetails.isCreature && views[i].creatureDetails != null) 
            newDetails.creature =  views[i].creatureDetails.Create(newDetails);
        else newDetails.creature = null;

        // spawn with a random hunger amt, only applies to creature hosts.
        //newDetails.currentHungerStage = UnityEngine.Random.Range(0,creatureHungerCycle); 

        // will need to set up what their starting condition is.
        newDetails.condition = (int)views[i].startingCondition; // set condition as starting condition, will want to load stuff after this.

        newDetails.sporeSpotAmt = views[i].sporeSpots.Length; // add slots equal to the amount of mushrooms length.
        newDetails.mushrooms = new MushroomInstance[newDetails.sporeSpotAmt]; // set up mushroom slots
        newDetails.mushroomsIDs = new SerializableGuid[newDetails.sporeSpotAmt]; // set up their IDs

        for(int b = 0; b < newDetails.sporeSpotAmt; b++) // set up slot ids
        {
            newDetails.mushroomsIDs[b] = views[i].sporeSpots[b].Id; // set slot id to the id of the views slot.
            if(newDetails.mushrooms[b]!=null) {
                newDetails.mushrooms[b].Id = newDetails.mushroomsIDs[b]; // set the slot as this id.
                newDetails.mushrooms[b].sporeIndex = views[i].sporeSpots[b].indexLocation;
            }
        }

        // Might want to add the details to the game object as a ref just so we can see it while in editor
        
        hosts[i] = newDetails; // add to host array.
        views[i].details = hosts[i];
    }

    // Used in conjunction with the save system
    public void LoadHostData(int i)
    {
        if (i<SaveManager.Instance.saveData.hosts.Length && SaveManager.Instance.saveData.hosts[i] != null && SaveManager.Instance.saveData.hosts[i].viewID != SerializableGuid.Empty)
        {   // Load In
            hosts[i] = SaveManager.Instance.saveData.hosts[i];
            SaveManager.Instance.saveData.hosts[i] = null;

            if (views[i] == null)
            {
                HostView target = HostViewsDictionary.GetDetailsById(hosts[i].veiwType);
                if (target != null)
                {
                    GameObject View;

                    View = Instantiate(target.gameObject,deadHostSlotPositions[i-deadHostsIndexDivider].transform);
                    
                    HostView tempView = View.GetComponent<HostView>();

                    tempView.ID = SerializableGuid.NewGuid(); // make guid new incase of prefab use

                    views[i] = tempView;
                    
                }
                else
                {
                    Debug.Log("Unrecognized Host at " + i);
                    return;
                }
                
            }

            views[i].managerIndex = i; // set index refrence
            views[i].Initialize(); // initialize in a more controlled fassion than their own start.
            views[i].details = hosts[i];
            views[i].OnConditionChange(hosts[i].condition);
            //Debug.Log(i);
            foreach(MushroomInstance mush in hosts[i].mushrooms)
            {
                if(mush.sporeIndex < views[i].sporeSpots.Length && mush != null && mush.Id != SerializableGuid.Empty && mush.details != null) // && mush.details.StagePrefabs[mush.currentStage] != null
                {
                    if(mush.currentStage>mush.details.MaxStageAmt) mush.currentStage = mush.details.MaxStageAmt;
                    //mush.currentStage--;
                    if(mush.currentStage<0) mush.currentStage = 0;
                    //Debug.Log("Sp: "+ mush.sporeIndex+ "  " + i);
                    MushroomDetails dets = MushroomDictionary.GetDetailsById(mush.dataId);
                    views[i].ChangeSporeSpotModel(mush.sporeIndex, dets.StagePrefabs[mush.currentStage]);
                    //mush.currentStage++;
                    //views[i].ChangeSporeSpotModel(mush.sporeIndex, mush.details.StagePrefabs[mush.currentStage]);
                    if(mush.currentStage == mush.details.HarvestableStage)
                        views[i].SetSporeSpotInteractivity(mush.sporeIndex,SporeSpotState.Harvestable);
                    else views[i].SetSporeSpotInteractivity(mush.sporeIndex,SporeSpotState.Growing);
                }
            }
        }
        else
        {   // New Begining
            RemoveHost(i);
        }
    }

    public void ProgressTimeState(int stages) /////////////////////////////////////////////////////////// Progress Time
    {
        if(hosts==null || hosts.Length<1) return; // catch for if not set up corectly

        foreach(HostDetails host in hosts)       /// Update Hosts
        {// chose foreach because its easier to type and we have a ref to index in host anyways
            
            if(host==null || host.condition < 0) continue; // skip if non existant or unusable
            // might want to add a effect that will clear if this is < 0; or do that in the body. 

            for(int i = 0; i < stages; i++){
                // EFFECTS On CONDITION
                int conditionEffect = 0; // will be added to host's condition
                int mushIndex = -1;
                if(host.mushrooms!=null){
                    foreach(MushroomInstance mush in host.mushrooms)       ///////// Mushrooms 
                    {
                        mushIndex++;
                        if(mush == null || mush.details == null) {
                            //Debug.Log("ChanceSpreadSpores"); 
                            ChanceSpreadSpores(host,mushIndex);
                            continue; // skip if this slot is empty
                        }
                        // add mushroom's condition effect to the tree's condition
                        int shroomEffect = mush.details.conditionEffect;

                        if (mush.nurtured)
                        {
                            shroomEffect /= 2;

                            // 1 in 5 chance to become un nurtured
                            int rand = UnityEngine.Random.Range(1,6);
                            if(rand == 5) mush.nurtured = false;
                        }

                        conditionEffect += shroomEffect;

                        if(mush.currentStage < mush.details.MaxStageAmt) // update stage of Mushroom
                        {
                            int newStage = UpdateMushroomStage(mush);
                            if (newStage > mush.currentStage)
                            {
                                mush.currentStage = newStage;
                                if(mush.currentStage >= mush.details.HarvestableStage) mush.harvestable = true;

                                // effect the view in some way. // like updating the prefab
                                if(mush.details.StagePrefabs.Length > newStage && mush.details.StagePrefabs[newStage] != null)
                                    views[host.index].ChangeSporeSpotModel(mush.sporeIndex, mush.details.StagePrefabs[newStage]);
                                
                            }
                            if (newStage >= mush.details.HarvestableStage)
                            {
                                views[host.index].SetSporeSpotInteractivity(mush.sporeIndex,SporeSpotState.Harvestable);
                            }
                        }

                        // update spore spot in some way to reflect any changes here.
                    }
                }

                UpdateHostCondition(host,conditionEffect);
            }
        }

        //RefreshEdibleList(); // refreshes it after each update
    }

    
    public void UpdateHostCondition(HostDetails host, int conditionEffect)
    {
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

        host.condition = newCondition;
        if(views[host.index]!=null)views[host.index].OnConditionChange(newCondition);
    }

    public int UpdateMushroomStage(MushroomInstance mush) /////////////////////////////////////////////////////// Update Mushroom State
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
        newMush.sporeIndex = views[hostIndex].sporeSpots[sporeSpotIndex].indexLocation;
        hosts[hostIndex].mushrooms[sporeSpotIndex] = newMush; // add mushroom to slot

        // do some visual shit with the view
        views[hostIndex].ChangeSporeSpotModel(sporeSpotIndex, hosts[hostIndex].mushrooms[sporeSpotIndex].details.StagePrefabs[0]);
        views[hostIndex].ChangeInteractTime(sporeSpotIndex, details.BaseHarvestTime);
        views[hostIndex].SetSporeSpotInteractivity(sporeSpotIndex,SporeSpotState.Growing);
    }  

    // Overload to add a random mushroom to the spot, of avalible ones.
    public void NewMushroomAtSporeSpot(int hostIndex, int sporeSpotIndex)
    {
        MushroomDetails tempDetails = SpawnRandomSporeableMushroom(views[hostIndex].sporeableMushrooms[sporeSpotIndex]);
        NewMushroomAtSporeSpot(hostIndex,sporeSpotIndex,tempDetails); 
    }

    public void NewMushroomAtSporeSpot(int hostIndex)
    {
        if (hosts[hostIndex] == null)
        {
            Debug.Log("New Mush Failed because no host is here");
            return;
        }

        foreach(MushroomInstance mush in hosts[hostIndex].mushrooms)
        {
            if (mush != null && mush.details!=null && mush.dataId != SerializableGuid.Empty)    
            {
                NewMushroomAtSporeSpot(hostIndex,mush.sporeIndex);
                return;
            }
        }
    }

    public void MushroomRemoved(int hostIndex, int sporeIndex) ///////////////////////////////////////////////// Mushroom Removed
    {
        if(hostIndex>=hosts.Length || hosts[hostIndex]==null ||  hosts[hostIndex].mushrooms.Length <= sporeIndex) {
            Debug.Log("Did not remove Mushroom");
            return;}

        Debug.Log("Mushroom Removed at " + hostIndex+" "+sporeIndex);

        hosts[hostIndex].mushrooms[sporeIndex] = null;
        // Make sure that when this is set to null the dataID is set to 0000000000 / empty
        // or create a mushroom details that is just empty that can be refrenced as such

        //Update view's spore spots.
        views[hostIndex].RemoveSporeSpotModel(sporeIndex);
        views[hostIndex].ChangeInteractTime(sporeIndex, -1);
        views[hostIndex].SetSporeSpotInteractivity(sporeIndex,SporeSpotState.Sporeable);

    }

    public void MushroomHarvested(int hostIndex, int sporeIndex) ///////////////////////////////////////////////// Mushroom Harvested
    {
        if(hostIndex>=hosts.Length || hosts[hostIndex]==null) return;

        hosts[hostIndex].mushrooms[sporeIndex].harvestable = false;
        hosts[hostIndex].mushrooms[sporeIndex].currentStage = 0;

        views[hostIndex].ChangeSporeSpotModel(sporeIndex, hosts[hostIndex].mushrooms[sporeIndex].details.StagePrefabs[0]);
        views[hostIndex].SetSporeSpotInteractivity(sporeIndex,SporeSpotState.Growing);
    }

    public MushroomDetails SpawnRandomSporeableMushroom(SporeableMushrooms_SO sm) /////////////////////////////////// Spawn Random Mushroom 
    {
        if(sm == null || sm.SporeableMushrooms == null || sm.SporeableMushrooms.Length < 1) return null;
        int randy = UnityEngine.Random.Range(0,sm.SporeableMushrooms.Length);

        return sm.SporeableMushrooms[randy];
    }

    public bool ChanceSpreadSpores(HostDetails host,int index) ///////////////////////////////////////////////////// Chance to spread Spores
    {
        int randChance = UnityEngine.Random.Range(0,101);
        if (randChance < chanceToSpore)
        {
            // add mushroom of type that is able to be spawned
            NewMushroomAtSporeSpot(host.index,index);
            return true;
        }
        else return false;
    }

    public void RefreshEdibleList() ///////////////////////////////////////////////////////////////////////////////// Refresh Edible List
    {
        edibleList.Clear();
        foreach(HostDetails host in hosts)
        {
            if(host == null || host.sporeSpotAmt<=0) continue; // skip if nothing is here
            foreach(MushroomInstance mush in host.mushrooms)
            {
                if(mush != null && mush.details!=null&&mush.dataId != SerializableGuid.Empty)
                {
                    edibleList.Add(mush);
                }
            }
        }
        // sort by greatest priority
        edibleList.Sort((mush1,mush2)=>mush2.currentPriority.CompareTo(mush1.currentPriority));
    }

    public void SetMushroomPriority(int hostIndex, int mushIndex, int bonus)
    {
        if(hosts.Length<hostIndex && hosts[hostIndex].mushrooms.Length < mushIndex){
            hosts[hostIndex].mushrooms[mushIndex].AddPriorityBonus(bonus);
        }
    }
    
    // Add A function that will add a new host view to the array, and then call setHostAtIndexI at that array position in hosts.
    // Eithier wilol be called from here or more likeley outside of here. if in here we'll need to instantiate a prefab in the world at a postion. 
    public bool AddViewToArray(int i, HostView newView)
    {
        if(i>= views.Length || views[i]!=null){
            Debug.Log("View " +i+ " is longer than the array");
            return false;
        }
        GameObject View;
        
        if(i>=deadHostsIndexDivider && i < creatureHostsIndexDivider)
        {
            //Debug.Log(i);
            float rad = CreatureManager.Instance.spawnRadius;
            if(CreatureManager.Instance.TryGetRandomNavMeshPoint(transform.position,rad,out Vector3 position)){
                float randomY = UnityEngine.Random.Range(0f, 360f);
                View = Instantiate(newView.gameObject,position,Quaternion.Euler(0f, randomY, 0f));
            }
            else
            {
                Debug.Log("Was Not able to find place on navmesh");
                return false;
            }
            //deadHostSlotPositions[i-deadHostsIndexDivider].transform
        }
        else
        {
            View = newView.gameObject;
        }
        
        HostView tempView = View.GetComponent<HostView>();

        tempView.ID = SerializableGuid.NewGuid(); // make guid new incase of prefab use
        views[i] = tempView;
        SetHostAtIndexI(i);

        return true;
    }
    public bool AddDeadViewToArray(HostView newView)
    {
        bool returnable = false;
        for(int i = deadHostsIndexDivider; i < creatureHostsIndexDivider; i++)
        {
            if (!returnable)
            {
                returnable = AddViewToArray(i,newView);
            }
        }
        return returnable;
    }
    public bool AddCreatureViewToArray(HostView newView)
    {
        bool returnable = false;
        for(int i = creatureHostsIndexDivider; i < views.Length; i++)
        {
            if (!returnable)
            {
                returnable = AddViewToArray(i,newView);
            }
        }
        return returnable;
    }

    public void RemoveHost(int i)
    {
        //views[i].destroyOnInvis = true;
        if(views[i] == null) return;
        if (views[i].isCreature)
        {
            CreatureManager.Instance.RemoveCreatureFromForest(i-creatureHostsIndexDivider);
        }
        Destroy(views[i].gameObject);
        views[i] = null;
        SetHostAtIndexI(i);
    }

    public void WaterShroom(int hostIndex, int sporeIndex)
    {
        if(hostIndex>hosts.Length || hosts[hostIndex]==null 
        || sporeIndex > hosts[hostIndex].sporeSpotAmt 
        || hosts[hostIndex].mushrooms[sporeIndex]==null) return;

        hosts[hostIndex].mushrooms[sporeIndex].nurtured = true;
    }

    public bool ShoomHarvested(int hostIndex, int sporeIndex)
    {
        if(hostIndex>hosts.Length || hosts[hostIndex]==null 
        || sporeIndex > hosts[hostIndex].sporeSpotAmt 
        || hosts[hostIndex].mushrooms[sporeIndex]==null) return false;

        if(hosts[hostIndex].mushrooms[sporeIndex].details.item!=null) {
            Item newItem = hosts[hostIndex].mushrooms[sporeIndex].details.item.Create(1);
            return invData.items.TryAdd(newItem);
        }
        else return true;
    }   
}

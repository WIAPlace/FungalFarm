using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI; // Required for NavMesh classes

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

    [SerializeField] private GameObject[] possibleCreatures;

    [Header("Spawning Details")]
    [SerializeField] private int ShroomsNeededToSpawn=10;
    [SerializeField] public float spawnRadius = 20f;
    [SerializeField] private int maxSpawnAttempts = 10;
    [SerializeField] private int chanceForCreature;
    [SerializeField] private int maxHungerDamage=5; 


    private void Start()
    {
        TimeManager.Instance.ManageTimer(this); // adds this to managed timers.
    }
    // the bool is to let us know if it worked
    public bool TryAddCreatureToForest()  ////////////////////////////////////////////////// Try Add
    {
        Debug.Log("TryingToAdd");
        for(int i = 0; i < creatures.Length; i++)
        {
            if(creatures[i]!=null && creatures[i].data!=null && creatures[i].ID!=SerializableGuid.Empty) continue;
            Debug.Log(i);
            SpawnCreatureRandomly(i);
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

            if (edibleList.Count >= ShroomsNeededToSpawn)
            {
                int chance = UnityEngine.Random.Range(1,chanceForCreature);

                if(chance == 1)
                {
                    TryAddCreatureToForest();
                }
            }

            foreach(CreatureInstance creat in creatures)
            {
                if(creat == null || creat.data==null||creat.dataID == SerializableGuid.Empty) continue;
                //Debug.Log("HungerRising");
                creat.currentHungerStage += UnityEngine.Random.Range(-1,3);
                if(creat.currentHungerStage<=0) creat.currentHungerStage = 0;
                
                if(creat.currentHungerStage >= creatureHungerCycle)
                {
                    int randomHungerPain = UnityEngine.Random.Range(maxHungerDamage,1);
                    HostManager.Instance.UpdateHostCondition(creat.host, randomHungerPain);
                    // eat the one of highest priority
                    if(edibleList==null && edibleList[0]==null) continue;
                    if(edibleList.Count<=0) continue;
                    if(TimeManager.Instance.currentIndex<=4) continue; // only eat at night

                    EdibleEffect effect = EdibleEffect.Neutral;



                    if(edibleList[0].details!=null) effect= edibleList[0].details.edibleEffect;

                    if(effect == EdibleEffect.Infected)
                    {
                        HostManager.Instance.NewMushroomAtSporeSpot(creat.host.index);
                    }

                    HostManager.Instance.UpdateHostCondition(creat.host, edibleList[0].details.conditionEffect);

                    HostManager.Instance.MushroomRemoved(edibleList[0].host.index,edibleList[0].sporeIndex);
                    edibleList.RemoveAt(0);
                    
                    // look through switch statment to have effects occure
                    // maybe if it gets death a number of times it dies, though it would be nice if it was tougher for certain shrooms
                }
            }
        }
    }

    public int RandPossibleCreatureIndex()
    {
        return UnityEngine.Random.Range(0,possibleCreatures.Length);
    }


    public void SpawnCreatureRandomly(int i)
    {
        Vector3 spawnPosition;

        if (TryGetRandomNavMeshPoint(transform.position, spawnRadius, out spawnPosition))
        {
            int spawnedIndex = RandPossibleCreatureIndex();
            // 1. Spawn the creature at the validated position
            GameObject spawnedCreature = Instantiate(possibleCreatures[spawnedIndex], spawnPosition, Quaternion.identity);

            CreatureInstance dets;
            HostView view;
            if(!spawnedCreature.TryGetComponent<HostView>(out view))
            {
                Debug.Log("Failed to get creature HostView");
                return;
            }
            if (!view.isCreature)
            {
                Debug.Log("Not a creature");
                return;
            }
            if (!HostManager.Instance.AddViewToArray(i+HostManager.Instance.creatureHostsIndexDivider,view))
            {
                Debug.Log("Failed To add Host To array");
                return;
            }

            
            if(view.details.creature!=null)dets = view.details.creature;
            else{ 
                Debug.Log("View.details.Creature is null");
                return;
            }
            
            
            
            // 2. CRITICAL: If the prefab has a NavMeshAgent, warp it to initialize safely
            NavMeshAgent agent = spawnedCreature.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(spawnPosition);
            }
            else
            {
                HostManager.Instance.RemoveHost(i);
                Debug.Log("Was Not Able to get Navmesh Agent");
                return;
            } 
            creatures[i] = dets;

        }
        else
        {
            Debug.LogWarning("Failed to find a valid NavMesh point after multiple attempts.");
        }
    }

    public bool TryGetRandomNavMeshPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Generate a random point inside a sphere zone
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * radius;
            randomPoint.y = center.y;
            NavMeshHit hit;

            // SamplePosition looks for the nearest valid point on the NavMesh within a max distance (e.g., 2.0f)
            if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    // Optional: Visualize the spawn boundary zone in the Unity Editor Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

}

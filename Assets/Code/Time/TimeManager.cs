using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

// Public enum for what time it is.
public enum TimeShifts
{
    Midnight,
    Dawn,
    Morning,
    Noon,
    Evening,
    Twilight,
    Dusk
}

/// <summary>
///  Time Manager class that controls what time we are on
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        // 2. Check if an instance already exists in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        // 3. Set the active global instance
        Instance = this;
    }

    public static float TotalGameTime;
    public float secondsToPassForeTick;
    public GameObject Owl;
    public GameObject Basket;
    public int TimeForOwl;
    //public event Action<int> TimeEvent;
    [SerializeField] private Material[] skyboxes;

    [field:SerializeField] public List<IOnTime> timers = new();

    void Start()
    {
        StartCoroutine(PassTimeOverTime());
    }



    public void ManageTimer(IOnTime managedTimer)
    {
        timers.Add(managedTimer);
    }

    public void PassTime(int stages)
    {
        PassingTime();
        if(timers == null || timers.Count < 1) return;

        for(int i = 0; i < stages; i++)
        {
            foreach(IOnTime managed in timers)
            {
                if(managed == null) continue;
                managed.ProgressTimeState(1);
            }
        }
    }
    int currentIndex;
    IEnumerator PassTimeOverTime()
    {
        while(true){
            yield return new WaitForSeconds(secondsToPassForeTick);
            PassTime(1);
            
        }
    }

    private void PassingTime()
    {
        int index = (currentIndex+1) % skyboxes.Length;
        currentIndex = index;
        if(skyboxes[currentIndex]!=null) RenderSettings.skybox = skyboxes[currentIndex];
        if(currentIndex == TimeForOwl)
        {
            Owl.SetActive(true);
           Basket.SetActive(true);
        }
        else if (Owl.activeSelf)
        {
           Owl.SetActive(true);
           Basket.SetActive(true);
        }
    }

    
}

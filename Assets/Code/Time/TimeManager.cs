using System;
using System.Collections.Generic;
using UnityEngine;

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

    //public event Action<int> TimeEvent;

    [field:SerializeField] public List<IOnTime> timers = new();

    public void ManageTimer(IOnTime managedTimer)
    {
        timers.Add(managedTimer);
    }

    public void PassTime(int stages)
    {
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

    
}

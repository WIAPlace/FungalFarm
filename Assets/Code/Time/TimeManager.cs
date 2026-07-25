using System;
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

    public event Action<int> TimeEvent;

    
}

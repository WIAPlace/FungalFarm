using System.Collections.Generic;
using UnityEngine;

// where all of the logic is held.
public class HostManager : MonoBehaviour
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
    public HostDetails[] hosts;
    public HostView[] hostViews;

    private void Start()
    {
        InitializeDetailsToViews();
    }

    public void InitializeDetailsToViews()
    {
        for(int i = 0; i < hostViews.Length; i++)
        {
            hostViews[i].managerIndex = i;
            HostDetails newDetails = new();
            newDetails.viewID = hostViews[i].ID;
            // will need to set up what their starting condition is.

            hosts[i] = newDetails;
        }
    }

}

using System.Collections.Generic;
using UnityEngine;


// attach to  the game object host. will act as the interface between the player and the controller.
public class HostView : MonoBehaviour
{
    [field:SerializeField] public SerializableGuid ID = SerializableGuid.NewGuid();
    public int managerIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

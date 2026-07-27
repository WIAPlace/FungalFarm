using UnityEngine;

// the conditions of the host matter for the fungi's ability to live
public enum HostCondition
{
    Unuseable,
    Decaying,
    Dying,
    Alive,
    Well
}


// represents what the mushroom is connected to, like a tree or the ground
// think of it like a plot of land in a usual farming game
public class Host : MonoBehaviour
{
    public HostCondition condition; // might be turned into an enum,
    public SporeableSpotInteractable[] sporeSpots; // positions where a mushroom can grow, and if multible can grow on this Host
    private MushroomDetails[] mushrooms;

    

    public void Awake()
    {
        
        
        // set up the spore spots index locations
        if(sporeSpots!=null){
            for(int i = 0; i < sporeSpots.Length; i++)
            {
                if(sporeSpots[i]!=null) sporeSpots[i].indexLocation = i;
            }
        }
    }
}   

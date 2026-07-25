using UnityEngine;

public interface IOnTime
{
    // int will be used usualy as one as the day goes on, but uped deppending on rest time.
    public void ProgressTimeState(int stages); 
}

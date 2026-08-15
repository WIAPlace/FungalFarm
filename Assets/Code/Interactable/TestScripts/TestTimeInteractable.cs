using UnityEngine;

public class TestTimeInteractable : MonoBehaviour,IInteractable
{
    public float timeToWait;
    public int stagesPassed;

    public void BeginInteract(out float waitTime,out float staminaDrain,ref InteractionType type)
    {
        waitTime = timeToWait;
        staminaDrain = 0;
        type = InteractionType.Basic; 
    }

    public void EndInteract(float currentWait, ref InteractionType type)
    {
        TimeManager.Instance.PassTime(stagesPassed);
        Debug.Log("Update");
    }
}

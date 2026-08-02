using UnityEngine;

public class TestTimeInteractable : MonoBehaviour,IInteractable
{
    public float timeToWait;
    public int stagesPassed;

    public void BeginInteract(out float waitTime)
    {
        waitTime = timeToWait;
    }

    public void EndInteract(float currentWait)
    {
        TimeManager.Instance.PassTime(stagesPassed);
    }
}

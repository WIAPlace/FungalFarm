using Unity.VisualScripting;
using UnityEngine;

public class FungalEventManager : MonoBehaviour, IOnTime
{
    [SerializeField] ParticleSystem ps;
    ParticleSystem.EmissionModule em;
    [SerializeField] private float worldSporeIncrease;

    private void Start()
    {
        TimeManager.Instance.ManageTimer(this); // adds this to managed timers.
        em = ps.emission;

        int shrooms = HostManager.Instance.edibleList.Count;
        em.rateOverTime=worldSporeIncrease*shrooms;
    }
    

    public void ProgressTimeState(int stages)
    {
        int shrooms = HostManager.Instance.edibleList.Count;

        em.rateOverTime=worldSporeIncrease*shrooms;
    }
}

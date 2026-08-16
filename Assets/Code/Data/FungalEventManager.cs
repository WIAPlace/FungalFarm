using System;
using Unity.VisualScripting;
using UnityEngine;

public class FungalEventManager : MonoBehaviour, IOnTime
{
    [SerializeField] ParticleSystem ps;
    ParticleSystem.EmissionModule em;
    [SerializeField] private float worldSporeIncrease;
    [SerializeField] private int leshRequiremnt;
    [SerializeField] GameObject lesh;
    [SerializeField] GameObject Heart;
    public float baseScale;
    public float heartScaleMulti;

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

        if (shrooms >= leshRequiremnt && TimeManager.Instance.currentIndex > 4 && lesh!=null)
        {
            lesh.SetActive(true);
        }
        else if(lesh!=null&&lesh.activeSelf)
        {
            lesh.SetActive(false);
        }

        if(Heart!=null)Heart.transform.localScale=Vector3.one*(baseScale+(shrooms*heartScaleMulti));
    }
}

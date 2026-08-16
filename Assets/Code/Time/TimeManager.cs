using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

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


[Serializable]
public class SkyValueHolder
{
    public Color SkyColor;
    public Color HorizonColor;
    public float light;
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

        Owl.SetActive(false);
        Basket.SetActive(false);
        //Day.mute=false;
        //night.mute=true;
    }

    public static float TotalGameTime;
    public float secondsToPassForeTick;
    public GameObject Owl;
    public GameObject Basket;
    public int TimeForOwl;
    private Coroutine time;
    [SerializeField] GameObject mainLight;
    //public event Action<int> TimeEvent;
    [SerializeField] private Material[] skyboxes;
    [SerializeField] private SkyValueHolder[] values;

    float mainLightY;
    float mainLightZ;
    [SerializeField] float LightDirChangeSpeed=3;

    [SerializeField] private float duration = 2.0f;

    [SerializeField] private float SunSize = 2.0f;
    [SerializeField] private float MoonSize = 2.0f;
    public Light baseLight;
    private float currentLight;
    public float lightChangeSpeed;
    private float targetlight;
    

    float timer;

    [SerializeField] private Material targetMat;
    private string skyColorReference = "_SkyColor";
    private string horizonColorReference = "_HorizonColor";
    private string sunSizeRef = "_SunSize";
    private Coroutine skyColorTransitionCoroutine;
    private Coroutine horizonColorTransitionCoroutine;

    //public AudioSource night;
    //public AudioSource Day;

    Color targetSkyColor;
    Color targetHorizonColor;

    Color currentSkyColor;
    Color currentHorizonColor;

    [field:SerializeField] public List<IOnTime> timers = new();

    void Start()
    { 
        time = StartCoroutine(PassTimeOverTime());
        mainLightY = mainLight.transform.rotation.y;
        mainLightZ = mainLight.transform.rotation.z;
        currentIndex = 0;
        PassingTime();
    }

    private void Update()
    {
        TranslateLight();
    }


    public void ManageTimer(IOnTime managedTimer)
    {
        timers.Add(managedTimer);
    }

    public void PassTime(int stages)
    {
        PassingTime();

        if(time != null) // reset time passing
        {
            StopCoroutine(time);
            time=StartCoroutine(PassTimeOverTime());
        }

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
    public int currentIndex;
    Quaternion targetRotation;

    IEnumerator PassTimeOverTime()
    {
        while(true){
            yield return new WaitForSeconds(secondsToPassForeTick);
            PassTime(1);
            
        }
    }

    private void PassingTime()
    {
        int index = (currentIndex+1) % skyboxes.Length;
        currentIndex = index;
        TurnLigth();
        //if(skyboxes[currentIndex]!=null) RenderSettings.skybox = skyboxes[currentIndex];

        if(currentIndex == TimeForOwl)
        {
            Owl.SetActive(true);
           Basket.SetActive(true);
        }
        else if (Owl.activeSelf)
        {
           Owl.SetActive(false);
           Basket.SetActive(false);
        }
    }

    private void TranslateLight()
    {
        if(mainLight.transform.rotation!=targetRotation){
            mainLight.transform.rotation = Quaternion.Slerp(mainLight.transform.rotation, targetRotation, LightDirChangeSpeed * Time.deltaTime);
            baseLight.intensity = Mathf.Lerp(baseLight.intensity,values[currentIndex].light,lightChangeSpeed *Time.deltaTime);
        }
    }

    private void TurnLigth()
    {
        switch (currentIndex)
        {
            case 0: // morning
                //Day.mute = false;
                //night.mute=true;
                targetMat.SetFloat(sunSizeRef, SunSize);
                targetRotation =  Quaternion.Euler(164.5f,  80f , 0);
                
                if(values[currentIndex]!=null){
                    StartLightTransition(values[currentIndex]);
                }

            break;

            case 1:
                targetRotation  =  Quaternion.Euler(137.1f,   80f , 0);
                if(values[currentIndex]!=null)StartLightTransition(values[currentIndex]);
            break;

            case 2:
                targetRotation  =  Quaternion.Euler(57.5f,   80f , 0);
                if(values[currentIndex]!=null)StartLightTransition(values[currentIndex]);
            break;

            case 3:
                targetRotation  =  Quaternion.Euler(9.4f,   80f , 0);
                if(values[currentIndex]!=null)StartLightTransition(values[currentIndex]);
            break;

            case 4: // night
                //Day.mute = true;
                //night.mute=false;
                targetMat.SetFloat(sunSizeRef, MoonSize);
                targetRotation  =  Quaternion.Euler(20.7f,   80f , 0);
                if(values[currentIndex]!=null)StartLightTransition(values[currentIndex]);
            break;

            case 5:
                targetRotation  =  Quaternion.Euler(76.58f,  80f , 0);
                if(values[currentIndex]!=null)StartLightTransition(values[currentIndex]);
            break;

            case 6:
                targetRotation  =  Quaternion.Euler(169.6f,   80f , 0);
                if(values[currentIndex]!=null)StartLightTransition(values[currentIndex]);
            break;

            default:
            break;
        }
    }

    public void StartLightTransition(SkyValueHolder value)
    {
        StartSkyColorTransition(value.SkyColor);
        StartHorizonColorTranisiton(value.HorizonColor);
    }
    public void StartSkyColorTransition(Color newColor)
    {
        // Stop any ongoing transition to prevent clipping
        if (skyColorTransitionCoroutine != null)
        {
            StopCoroutine(skyColorTransitionCoroutine);
        }
        

        // Start the new transition
        skyColorTransitionCoroutine = StartCoroutine(TransitionColorRoutine(newColor,skyColorReference));
    }
    public void StartHorizonColorTranisiton(Color newColor)
    {
        if (horizonColorTransitionCoroutine != null)
        {
            StopCoroutine(horizonColorTransitionCoroutine);
        }

        horizonColorTransitionCoroutine = StartCoroutine(TransitionColorRoutine(newColor,horizonColorReference));
    }
    

    private IEnumerator TransitionColorRoutine(Color targetColor, string refrenceName)
    {
        Color startColor = targetMat.GetColor(refrenceName);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpPercent = elapsedTime / duration;

            // Interpolate between the current color and the picked color
            Color currentLerpedColor = Color.Lerp(startColor, targetColor, lerpPercent);
            
            // Apply the color to the shader property
            targetMat.SetColor(refrenceName, currentLerpedColor);

            yield return null;
        }

        // Ensure the final color matches exactly
        targetMat.SetColor(refrenceName, targetColor);
    }
}

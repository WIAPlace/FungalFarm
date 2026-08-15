using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements; // Required namespace for UI Toolkit

public class ConditionCheck : MonoBehaviour
{
    [Header("UI Toolkit Reference")]
    [SerializeField] private UIDocument uiDocument;
    //[SerializeField] private string bg = "BackgroundBox";
    [SerializeField] private string backgroundElementName = "HostConditionBar";
    [SerializeField] private string fillElementName = "FillBar";
    [SerializeField] private string statusName = "Status";
    [SerializeField] private string hostName = "Host";

    [Header("Color Thresholds")]
    [SerializeField] private Color highConditionColor = new Color(0.2f, 0.8f, 0.2f); // Green
    [SerializeField] private Color mediumConditionColor = new Color(0.9f, 0.6f, 0.1f); // Orange
    [SerializeField] private Color lowConditionColor = new Color(0.8f, 0.2f, 0.2f); // Red

    [Header("Fade Settings")]
    [Tooltip("Minimum Condition change required in a single frame to keep the bar fully visible.")]
    [SerializeField] private float fadeSpeed = 4.0f;
    public float idleBeforeFade = 5;
    public float currentIdle = 0;

    private float currentCondition=100;
    public float maxCondition;

    private bool justChecked;
    private float newCondition;
    private Condition currentCon;
    private float conEffect;
    
    private float targetOpacity = 1f;

    
    private VisualElement ConditionBackground;
    private VisualElement ConditionBarFill;
    private Label status;
    private Label host;

    string nameOfHost;

    private void OnEnable()
    {
        // Fetch the target VisualElement from the UI Document root
        var root = uiDocument.rootVisualElement;
        ConditionBackground = root.Q<VisualElement>(backgroundElementName);
        ConditionBarFill = root.Q<VisualElement>(fillElementName);
        status = root.Q<Label>(statusName);
        host = root.Q<Label>(hostName);

        targetOpacity = 0f;
        currentIdle = idleBeforeFade;

        if (ConditionBackground != null)
        {
            ConditionBackground.style.opacity = 0f; 
        }
    }

    public void NewConditionInteracted(int condition,int maxCon,string name)
    {
        nameOfHost=name;
        maxCondition=maxCon;
        justChecked=true;
        conEffect = 0;

        currentCon = IntToCondition(condition);

        if(condition <= 0) newCondition = 0;
        else newCondition = condition;
        //Debug.Log(justChecked);
    }
    public void NewConditionInteracted(int condition,int maxCon,string name,float conEffect)
    {
        nameOfHost=name;
        maxCondition=maxCon;
        justChecked=true;
        this.conEffect = conEffect; 
        //currentCon = IntToCondition(condition);

        if(condition <= 0) newCondition = 0;
        else newCondition = condition;
        //Debug.Log(justChecked);
    }

    public void Update()
    {
        if(currentCondition != newCondition){
            currentCondition = newCondition;
            UpdateUI();
        }
        HandleFadeLogic();
    }

    private void HandleFadeLogic()
    {
        
        // If the change is larger than our threshold, it is active
        if (justChecked)
        {
            targetOpacity = 1f; // Instantly wake up and display UI
            justChecked = false;
            currentIdle =0;
        }
        else
        {
            if(currentIdle >= idleBeforeFade && targetOpacity!=0){
                // If change is below the threshold, fade out right away
                targetOpacity = 0f; 
            }
            else if( currentIdle < idleBeforeFade+1) currentIdle += Time.deltaTime;
        }

        // Apply smooth visual transparency interpolation
        if (ConditionBackground != null)
        {
            float currentOpacity = ConditionBackground.resolvedStyle.opacity;
            float tempSpeed;
            if(targetOpacity == 0) tempSpeed=fadeSpeed;
            else tempSpeed = fadeSpeed*2;
            ConditionBackground.style.opacity = Mathf.MoveTowards(currentOpacity, targetOpacity, tempSpeed * Time.deltaTime);
        }
    }
    

    private void UpdateUI()
    {
        if (ConditionBarFill == null) return;

        // Calculate standard 0 to 1 percentage ratio
        float ConditionRatio = currentCondition / maxCondition;

        if(maxCondition>=200) {
            status.text = "Condition: "+currentCon.ToString(); // if we are looking at a host
            host.text = nameOfHost;
        }
        else{
            if(conEffect<0) status.text = conEffect.ToString() + " To Host's Condition";
            else status.text = "+"+conEffect.ToString() + " To Host's Condition";

            if(ConditionRatio >= 1)
            {
                host.text = nameOfHost + " : Ready To Harvest";
            }
            else
            {
                host.text = nameOfHost+" : Growing";
            }
        }



        

        // Efficiently scale the fill element horizontally using IStyle
        ConditionBarFill.style.transformOrigin = new TransformOrigin(Length.Percent(0), Length.Percent(0));
        ConditionBarFill.style.scale = new Scale(new Vector3(ConditionRatio, 1f, 1f));

        // 2. Change colors dynamically based on ranges
        if (ConditionRatio < 0.5f)
        {
            ConditionBarFill.style.backgroundColor = Color.Lerp(lowConditionColor, mediumConditionColor, ConditionRatio * 2f);
        }
        else
        {
            ConditionBarFill.style.backgroundColor = Color.Lerp(mediumConditionColor, highConditionColor, (ConditionRatio - 0.5f) * 2f);
        }
    }

    public Condition IntToCondition(int con)
    {
        Condition newCon;
        if(con < 0) // unusable
        {
            newCon = Condition.unusable;
        }
        else if (con < 100)
        {
            newCon = Condition.dead;
        }
        else if (con < 200)
        {
            newCon = Condition.rotting;
        }
        else
        {
            newCon = Condition.healthy;
        }
        return newCon;
    }
}

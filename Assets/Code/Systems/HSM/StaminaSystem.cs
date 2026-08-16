using HSM;
using UnityEngine;
using UnityEngine.UIElements; // Required namespace for UI Toolkit

public class StaminaSystem : MonoBehaviour
{
    [Header("UI Toolkit Reference")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private string backgroundElementName = "StaminaBarBackground";
    [SerializeField] private string fillElementName = "StaminaBarFill";

    [Header("Color Thresholds")]
    [SerializeField] private Color highStaminaColor = new Color(0.2f, 0.8f, 0.2f); // Green
    [SerializeField] private Color mediumStaminaColor = new Color(0.9f, 0.6f, 0.1f); // Orange
    [SerializeField] private Color lowStaminaColor = new Color(0.8f, 0.2f, 0.2f); // Red

    [Header("Fade Settings")]
    [Tooltip("Minimum stamina change required in a single frame to keep the bar fully visible.")]
    [SerializeField] private float changeThreshold = 0.01f; 
    [SerializeField] private float idleDurationBeforeFade = 2.0f;
    [SerializeField] private float fadeSpeed = 4.0f;

    private float idleTimer;
    private float lastStamina=100;
    private float currentStamina=100;
    [HideInInspector]public float maxStamina;

    private float targetOpacity = 1f;

    private VisualElement staminaBackground;
    private VisualElement staminaBarFill;

    private void OnEnable()
    {
        // Fetch the target VisualElement from the UI Document root
        var root = uiDocument.rootVisualElement;
        staminaBackground = root.Q<VisualElement>(backgroundElementName);
        staminaBarFill = root.Q<VisualElement>(fillElementName);

        targetOpacity = 0f;
        idleTimer = idleDurationBeforeFade; // Pre-fill timer so it doesn't wait to stay hidden

        if (staminaBackground != null)
        {
            staminaBackground.style.opacity = 0f; 
        }
    }
    public void UpdateFadeThreshold(float newThreshold)
    {
        changeThreshold = newThreshold;
    }

    public void UpdateStaminaBar(float newStamina)
    {
        
        if(currentStamina != newStamina){
            currentStamina = newStamina;
            UpdateUI();
        }
        HandleFadeLogic();
    }

    private void HandleFadeLogic()
    {
        // Calculate exactly how much stamina changed this frame
        float frameDelta = Mathf.Abs(currentStamina - lastStamina);

        // If the change is larger than our threshold, it is active
        if (frameDelta > changeThreshold)
        {
            idleTimer = 0f;
            targetOpacity = 1f; // Instantly wake up and display UI
        }
        else
        {
            // If change is below the threshold, treat it as stagnant/idle
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDurationBeforeFade)
            {
                targetOpacity = 0f; // Start fading to transparent
            }
        }

        // Save current stamina for the next frame's comparison
        lastStamina = currentStamina;

        // Apply smooth visual transparency interpolation
        if (staminaBackground != null)
        {
            float currentOpacity = staminaBackground.resolvedStyle.opacity;
            staminaBackground.style.opacity = Mathf.MoveTowards(currentOpacity, targetOpacity, fadeSpeed * Time.deltaTime);
        }
    }
    

    private void UpdateUI()
    {
        if (staminaBarFill == null) return;

        // Calculate standard 0 to 1 percentage ratio
        float staminaRatio = currentStamina / maxStamina;

        // Efficiently scale the fill element horizontally using IStyle
        staminaBarFill.style.transformOrigin = new TransformOrigin(Length.Percent(0), Length.Percent(0));
        staminaBarFill.style.scale = new Scale(new Vector3(staminaRatio, 1f, 1f));

        // 2. Change colors dynamically based on ranges
        if (staminaRatio < 0.5f)
        {
            staminaBarFill.style.backgroundColor = Color.Lerp(lowStaminaColor, mediumStaminaColor, staminaRatio * 2f);
        }
        else
        {
            staminaBarFill.style.backgroundColor = Color.Lerp(mediumStaminaColor, highStaminaColor, (staminaRatio - 0.5f) * 2f);
        }
    }

}

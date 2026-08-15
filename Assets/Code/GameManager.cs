using System.Collections.Generic;
using HSM;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Enforce the singleton pattern: destroy duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: Keep this object alive across scene transitions
        //DontDestroyOnLoad(gameObject);
        var root = MoneyUI.rootVisualElement;
        moneyUIElement = root.Q<VisualElement>("Wallet");
    }

    [SerializeField] InputReader input;
    [SerializeField] UIController uiController;
    [SerializeField] FungiToSpore FungiMenu;
    [field: SerializeField] public PlayerStateDriver ctx;
    [SerializeField] ConditionCheck conCheck;
    [SerializeField] UIDocument MoneyUI;
    [SerializeField] ShopMenu shopMenu;
    public Money money;

    private VisualElement moneyUIElement;
    
    //public List<int> OpenContainers = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input.ResumeEvent += HandleResume;
        input.PauseEvent += HandlePause;
        input.InventoryEvent += HandleInventory;
        input.ShroomMenuEvent += HandleFungiMenu;

        ToggleWallet(false);
    }

    void OnDestroy()
    {
        input.ResumeEvent -= HandleResume;
        input.PauseEvent -= HandlePause;
        input.InventoryEvent -= HandleInventory;
        input.ShroomMenuEvent -= HandleFungiMenu;
    }


    private void HandlePause()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private void HandleResume()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        Time.timeScale = 1f;
        uiController.CloseAll();
        FungiMenu.CloseWindow();
        ToggleWallet(false);
        shopMenu.CloseShopUI();
    }
    private void HandleInventory()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 0f;
        input.SetUI();
        // will need to look into how to iterate through all containers to see all that are open then close them.
        uiController.ToggleWindow(UIController.Containers[0]);
        ToggleWallet(true);
        //uiController.ToggleWindow(UIController.Containers[1]);
    }
    public void OpenSpecificInventory(int index)
    {
        uiController.ToggleWindow(UIController.Containers[index]);
        HandleInventory();
    }

    public void ChangeMoneyBy(int amtChanged)
    {
        money.Amt+=amtChanged;
    }
    public void HandleFungiMenu()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 0f;

        FungiMenu.OpenWindow();
    }

    public MushroomDetails GetIntendedShroom()
    {
        return ctx.ctx.intendedSpore;
    }

    public void CheckConditionBar(int con,int max, string name)
    {
        //Debug.Log(con);
        conCheck.NewConditionInteracted(con,max,name);
    }
    public void CheckConditionBar(int con,int max,string name,float conEffect)
    {
        //Debug.Log(con);
        conCheck.NewConditionInteracted(con,max,name,conEffect);
    }
    public void ToggleWallet(bool condition)
    {
        if(!condition) moneyUIElement.style.display = DisplayStyle.None;
        else moneyUIElement.style.display = DisplayStyle.Flex;
    }
    public void ToggleShop(bool condition)
    {
        if(condition){
            shopMenu.ShowShopUI();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            Time.timeScale = 0f;
            input.SetUI();
            ToggleWallet(true);
        }

        else shopMenu.CloseShopUI();
    }
}

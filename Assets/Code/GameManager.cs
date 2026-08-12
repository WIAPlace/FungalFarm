using System.Collections.Generic;
using UnityEngine;

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
    }

    [SerializeField] InputReader input;
    [SerializeField] UIController uiController;
    public Money money;
    
    //public List<int> OpenContainers = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input.ResumeEvent += HandleResume;
        input.PauseEvent += HandlePause;
        input.InventoryEvent += HandleInventory;
    }

    void OnDestroy()
    {
        input.ResumeEvent -= HandleResume;
        input.PauseEvent -= HandlePause;
        input.InventoryEvent -= HandleInventory;
    }


    private void HandlePause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    private void HandleResume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        uiController.CloseAll();
    }
    private void HandleInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        input.SetUI();
        // will need to look into how to iterate through all containers to see all that are open then close them.
        uiController.ToggleWindow(UIController.Containers[0]);
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
}

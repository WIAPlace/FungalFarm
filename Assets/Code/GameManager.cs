using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] InputReader input;
    [SerializeField] UIController uiController;
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
        uiController.ToggleWindow("inventory");
        uiController.ToggleWindow("equipment");
    }
}

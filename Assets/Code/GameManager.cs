using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] InputReader input;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input.ResumeEvent += HandleResume;
        input.PauseEvent += HandlePause;
    }

    void OnDestroy()
    {
        input.ResumeEvent -= HandleResume;
        input.PauseEvent -= HandlePause;
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
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject healthBar;
    public static bool isPaused;
    public bool cursorLocked = true; // Track cursor lock state

    void Start()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor initially
        Cursor.visible = false; // Hide cursor initially
    }

    void Update()
    {
        if(!DieMenu.isDie) // Use the static reference instead of dieMenu.isDied
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        healthBar.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Make cursor visible
        cursorLocked = false;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        healthBar.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(1); // scene from project profile
        pauseMenu.SetActive(false);
        healthBar.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        DieMenu.isDie = false; // Reset die state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0); // scene from project profile
        isPaused = false;
        DieMenu.isDie = false; // Reset die state
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class DieMenu : MonoBehaviour
{
    public GameObject dieMenu;
    public GameObject healthBar;
    public static bool isDie;
    public bool cursorLocked = true; // Track cursor lock state

    void Start()
    {
        dieMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor initially
        Cursor.visible = false; // Hide cursor initially
    }

    void Update()
    {
        
    }

    public void Die()
    {
        dieMenu.SetActive(true);
        healthBar.SetActive(false);
        Time.timeScale = 0f;
        isDie = true;
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Make cursor visible
        cursorLocked = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(1); // scene from project profile
        dieMenu.SetActive(false);
        healthBar.SetActive(true);
        Time.timeScale = 1f;
        isDie = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0); // scene from project profile
        isDie = false;
    }
}

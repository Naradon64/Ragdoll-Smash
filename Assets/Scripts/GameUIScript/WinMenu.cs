using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public GameObject winMenu;
    public GameObject healthBar;
    public GameObject lockEnemyText;
    public static bool isDie;
    public bool cursorLocked = true; // Track cursor lock state

    void Start()
    {
        winMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor initially
        Cursor.visible = false; // Hide cursor initially
    }

    void Update()
    {
        
    }

    public void Win()
    {
        winMenu.SetActive(true);
        healthBar.SetActive(false);
        lockEnemyText.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None; // Unlock cursor
        Cursor.visible = true; // Make cursor visible
        cursorLocked = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(1); // scene from project profile
        winMenu.SetActive(false);
        healthBar.SetActive(true);
        lockEnemyText.SetActive(true);
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

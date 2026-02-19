using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject winScreenPanel;
    public GameObject gameOverPanel; // หน้าจอใหม่ตอนตาย

    private bool isPaused = false;
    private bool isGameEnded = false; // ใช้รวมกันทั้งตอน Win และ Game Over

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (winScreenPanel != null) winScreenPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameEnded)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // --- ระบบ Pause ---
    public void PauseGame()
    {
        if (isGameEnded) return; 

        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    public void RestartFromCheckpoint()
    {
        ResumeGame(); 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.ForceRespawn(); 
        }
    }

    // --- ระบบ Win Screen ---
    public void ShowWinScreen()
    {
        isGameEnded = true;
        if (winScreenPanel != null) winScreenPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    // --- ระบบ Game Over (ใหม่) ---
    public void ShowGameOver()
    {
        isGameEnded = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเวลาตอนตาย
    }

    public void RestartNewGame()
    {
        Time.timeScale = 1f;
        
        // ล้างค่า Checkpoint ทิ้ง ตัวละครจะได้ไปเกิดที่จุดเริ่มต้นด่านจริงๆ
        Checkpoint.ResetCheckpoint(); 
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        Checkpoint.ResetCheckpoint(); // ถ้าชนะแล้วกดเล่นใหม่ ก็ควรล้าง Checkpoint ด้วยเช่นกัน
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}
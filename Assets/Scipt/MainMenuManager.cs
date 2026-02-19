using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("ใส่ชื่อ Scene ด่านแรกของคุณตรงนี้ให้เป๊ะทุกตัวอักษร")]
    public string firstLevelName = "Level1"; 

    public void PlayGame()
    {
        // รีเซ็ต Time.timeScale เป็น 1 เสมอ 
        // เผื่อผู้เล่นกดกลับสู่เมนูหลักจากหน้า Pause แล้วเวลาถูกแช่แข็งไว้
        Time.timeScale = 1f; 
        
        // โหลดหน้าด่านแรก
        SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            // ออกจาก Play Mode ใน Unity Editor
            EditorApplication.isPlaying = false;
        #else
            // ปิดเกมจริงเวลารันเป็น Build
            Application.Quit();
        #endif
    }
}
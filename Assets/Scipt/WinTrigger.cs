using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    [Header("References")]
    public GameUIManager uiManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าคนที่มาชนคือ Player
        if (other.CompareTag("Player"))
        {
            if (uiManager != null)
            {
                uiManager.ShowWinScreen();
            }
            else
            {
                Debug.LogWarning("ยังไม่ได้ลาก GameUIManager มาใส่ในช่องอ้างอิงของ WinTrigger!");
            }
        }
    }
}
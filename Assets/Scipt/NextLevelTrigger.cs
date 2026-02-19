using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    [Header("Level Transition")]
    [Tooltip("ใส่ชื่อ Scene ของด่านต่อไปให้เป๊ะ (เช่น Map2)")]
    public string nextSceneName;

    private bool isLoading = false; // ป้องกันการเดินชนซ้ำแล้วโหลดฉากเบิ้ล

    void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าคนที่มาชนคือ Player และยังไม่ได้เริ่มโหลดฉาก
        if (other.CompareTag("Player") && !isLoading)
        {
            isLoading = true;
            Debug.Log("กำลังโหลดด่าน: " + nextSceneName);

            // โหลด Scene แบบ Async จะช่วยให้เกมบนมือถือไม่ค้างระหว่างเปลี่ยนด่าน
            SceneManager.LoadSceneAsync(nextSceneName);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using TMPro; // สำคัญมาก: ต้องเพิ่มบรรทัดนี้เพื่อใช้งาน TextMeshPro

public class StarDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public int requiredStars = 3; // จำนวนดาวที่ต้องเก็บให้ครบ
    private int currentStars = 0;

    [Header("UI Settings")]
    [Tooltip("ลาก UI Text ที่ต้องการแสดงตัวเลขมาใส่ตรงนี้")]
    public TextMeshProUGUI starText; // ตัวแปรสำหรับจับคู่กับข้อความบนหน้าจอ

    [Header("Events (Optional)")]
    public UnityEvent onDoorOpened;

    void Start()
    {
        // อัปเดตข้อความบนหน้าจอทันทีที่เริ่มด่าน (ให้โชว์เป็น 0/3)
        UpdateStarUI();
    }

    public void AddStar()
    {
        currentStars++;
        Debug.Log("เก็บดาวได้แล้ว: " + currentStars + " / " + requiredStars);

        // อัปเดตตัวเลขบนหน้าจอทุกครั้งที่เก็บดาวได้
        UpdateStarUI();

        if (currentStars >= requiredStars)
        {
            OpenDoor();
        }
    }

    private void UpdateStarUI()
    {
        // เช็คว่ามีการลาก UI Text มาใส่หรือยัง ป้องกัน Error
        if (starText != null)
        {
            // เปลี่ยนข้อความเป็น "STAR : ปัจจุบัน / เป้าหมาย" (เช่น "STAR : 1 / 3")
            starText.text = "STAR : " + currentStars.ToString() + " / " + requiredStars.ToString();
        }
        else
        {
            Debug.LogWarning("อย่าลืมลาก UI Text มาใส่ในช่อง Star Text ของ StarDoor นะครับ!");
        }
    }

    private void OpenDoor()
    {
        Debug.Log("ดาวครบแล้ว! ประตูเปิด!");
        
        onDoorOpened?.Invoke(); 
        gameObject.SetActive(false); 
    }
}
using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    [Header("References")]
    [Tooltip("ลาก GameObject ประตูในฉากมาใส่ตรงนี้")]
    public StarDoor targetDoor; 

    [Header("Effects")]
    public AudioClip collectSound; // เสียงตอนเก็บดาว

    void Start()
    {
        // ถ้าลืมลากประตูมาใส่ ให้สคริปต์ลองหาประตูในฉากอัตโนมัติ (ช่วยให้ทำงานง่ายขึ้น)
        if (targetDoor == null)
        {
            targetDoor = Object.FindFirstObjectByType<StarDoor>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าคนที่มาชนคือผู้เล่นใช่หรือไม่
        if (other.CompareTag("Player"))
        {
            if (targetDoor != null)
            {
                targetDoor.AddStar(); // ส่งสัญญาณไปบอกประตูว่าเก็บดาวได้ 1 ดวง
            }

            // เล่นเสียงเก็บดาว (สร้าง GameObject ชั่วคราวเพื่อเล่นเสียง เพราะดาวตัวนี้กำลังจะโดนลบ)
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // ทำลายดวงดาวทิ้ง
            Destroy(gameObject);
        }
    }
}
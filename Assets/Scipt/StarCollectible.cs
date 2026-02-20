using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    [Header("References")]
    [Tooltip("ลาก GameObject ประตูในฉากมาใส่ตรงนี้")]
    public StarDoor targetDoor; 

    [Header("Effects")]
    public AudioClip collectSound; // เสียงตอนเก็บดาว
    [Tooltip("ช่องเสียงเล่นเฉพาะเอฟเฟกต์เก็บดาว (Optional). หากไม่ใส่จะใช้ PlayClipAtPoint")]
    public AudioSource sfxSource;  // เพิ่มช่องให้ระบุ AudioSource สำหรับเล่นเสียงเอฟเฟกต์

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

            // เล่นเสียงเก็บดาว รองรับทั้งผ่าน AudioSource กับ PlayClipAtPoint
            if (collectSound != null)
            {
                if (sfxSource != null)
                {
                    sfxSource.PlayOneShot(collectSound);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);
                }
            }

            // ทำลายดวงดาวทิ้ง
            Destroy(gameObject);
        }
    }
}
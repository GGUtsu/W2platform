using UnityEngine;

public class Spride : MonoBehaviour
{
    // กับดักหนาม ตัวละครเดินชนแล้วหัวใจหายไป 1 ดวง (ใช้กับระบบ Health.cs)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // เรียกฟังก์ชัน TakeDamage ของ Health.cs เพื่อหักหัวใจ
                playerHealth.TakeDamage(1);
            }
        }
    }
}

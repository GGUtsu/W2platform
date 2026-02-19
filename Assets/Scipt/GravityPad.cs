using UnityEngine;

public class GravityPad : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ระยะเวลาดีเลย์ก่อนที่จะเหยียบซ้ำได้ เพื่อกันบั๊กสลับรัวๆ")]
    public float cooldown = 0.5f; 
    
    private float lastTriggerTime = -10f;

    void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าคนที่มาเหยียบคือ Player หรือไม่
        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
        {
            // เช็ค Cooldown
            if (Time.time >= lastTriggerTime + cooldown)
            {
                PlayerController player = other.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    player.ToggleGravity(); // สั่งสลับโลก
                    lastTriggerTime = Time.time;
                }
            }
        }
    }
}
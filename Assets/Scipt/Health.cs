using UnityEngine;
using UnityEngine.Events;
using System.Collections; // เพิ่มบรรทัดนี้เพื่อใช้งานระบบหน่วงเวลา (Coroutine)

public class Health : MonoBehaviour
{
    [Header("Heart Health Settings")]
    public int maxHearts = 3;
    public int currentHearts;

    [Header("Invulnerability After Hit")]
    public float invulnDuration = 1.0f;
    private float invulnTimer = 0f;
    private bool isInvulnerable = false;

    [Header("UI Events")]
    public UnityEvent<int> OnHealthChanged;

    [Header("Animation Delays")]
    [Tooltip("ระยะเวลาแอนิเมชันตาย ก่อนที่หน้า Game Over จะเด้งขึ้นมา")]
    public float deathAnimationTime = 1.2f;

    [Header("Player Death Sound")]
    [Tooltip("เสียง effect ที่จะเล่นตอนตัวละครตาย")]
    public AudioClip deathSound;

    [Header("Player Hurt Sound")]
    [Tooltip("เสียง effect ที่จะเล่นตอนเลือดลด")]
    public AudioClip hurtSound;

    private AudioSource audioSource;

    private Animator animator; // ตัวแปรสำหรับคุมแอนิเมชัน
    private PlayerController playerController; // ตัวแปรสำหรับสั่งหยุดเดินตอนตาย

    void Start()
    {
        currentHearts = maxHearts;
        OnHealthChanged?.Invoke(currentHearts);

        // ดึง Component Animator และ PlayerController มาเตรียมไว้
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        // เตรียม AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (deathSound != null || hurtSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isInvulnerable)
        {
            invulnTimer -= Time.deltaTime;
            if (invulnTimer <= 0f)
            {
                isInvulnerable = false;
            }
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isInvulnerable) return;

        currentHearts -= damage;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);

        OnHealthChanged?.Invoke(currentHearts);

        // เล่นเสียงเมื่อตัวละครโดนโจมตีและยังไม่ตาย
        if (currentHearts > 0)
        {
            if (hurtSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hurtSound);
            }

            // ยังไม่ตาย: สั่งเล่นแอนิเมชัน "เจ็บ" แล้ววาร์ปกลับจุดเกิด
            if (animator != null) animator.SetTrigger("Hurt");

            RespawnAtCheckpoint();
            BecomeInvulnerable(invulnDuration);
        }
        else
        {
            // ตายแล้ว: สั่งเล่นแอนิเมชัน "ตาย"
            if (animator != null) animator.SetTrigger("Die");

            // เล่นเสียงเมื่อตาย
            if (deathSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(deathSound);
            }

            // สั่งให้ตัวละครหยุดขยับ ล็อกการบังคับทุกอย่าง
            if (playerController != null) playerController.SetCanMove(false);

            // ป้องกันไม่ให้โดนตีซ้ำตอนนอนตาย
            isInvulnerable = true; 

            // เรียกใช้ Coroutine เพื่อหน่วงเวลารอให้แอนิเมชันตายเล่นจบ ค่อยโชว์หน้า Game Over
            StartCoroutine(WaitAndShowGameOver());
        }
    }

    // ฟังก์ชันหน่วงเวลา
    private IEnumerator WaitAndShowGameOver()
    {
        // รอเป็นเวลา deathAnimationTime วินาที
        yield return new WaitForSeconds(deathAnimationTime);

        // ค้นหา GameManager เพื่อเปิดหน้า Game Over และหยุดเวลา (Time.timeScale = 0)
        GameUIManager uiManager = Object.FindFirstObjectByType<GameUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
    }

    private void BecomeInvulnerable(float duration)
    {
        isInvulnerable = true;
        invulnTimer = duration;
    }

    public void RespawnAtCheckpoint()
    {
        Vector3 checkpoint = Checkpoint.GetSpawnPosition();
        transform.position = checkpoint;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void ForceRespawn()
    {
        RespawnAtCheckpoint();
    }
}

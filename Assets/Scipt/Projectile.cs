using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 30f; // ปรับตั้งต้นให้เร็วขึ้น (ปรับใน Inspector ได้)
    public int damage = 1;
    public float lifetime = 3f;
    
    [Tooltip("ติ๊กถูกถ้าเป็นกระสุนของศัตรู (จะทำดาเมจใส่ Player)")]
    public bool isEnemyProjectile = false; 

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime); 
    }

    public void Setup(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;

        // --- เพิ่มการหมุนหัวกระสุนให้พุ่งตรงไปตามทิศทางที่ยิง ---
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (isEnemyProjectile)
        {
            if (hitInfo.CompareTag("Player"))
            {
                Health playerHealth = hitInfo.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage); 
                }
                Destroy(gameObject);
            }
        }
        else
        {
            EnemyHealth enemyHealth = hitInfo.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
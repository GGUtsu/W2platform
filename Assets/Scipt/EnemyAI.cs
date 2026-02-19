using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Patrol")]
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    private bool movingRight = true;

    [Header("Chase Player")]
    public float chaseRange = 5f;
    public float chaseSpeed = 4f;
    private Transform player;

    [Header("Ranged Attack (Skill)")]
    public GameObject enemyProjectilePrefab;
    public Transform firePoint;
    public float attackRange = 8f;
    public float minShootInterval = 1.5f;
    public float maxShootInterval = 3.5f;
    private float shootTimer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        shootTimer = Random.Range(minShootInterval, maxShootInterval);
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                ShootSkill();
                shootTimer = Random.Range(minShootInterval, maxShootInterval);
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (leftPoint == null || rightPoint == null) return;

        float targetX = movingRight ? rightPoint.position.x : leftPoint.position.x;
        float dir = movingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

        if ((movingRight && transform.position.x >= targetX) || (!movingRight && transform.position.x <= targetX))
        {
            Flip();
        }
    }

    void ChasePlayer()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

        if (dir > 0 && transform.localScale.x < 0) Flip();
        else if (dir < 0 && transform.localScale.x > 0) Flip();
    }

    void ShootSkill()
    {
        if (enemyProjectilePrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(enemyProjectilePrefab, firePoint.position, Quaternion.identity);
            Projectile proj = bullet.GetComponent<Projectile>();

            // --- ปรับให้เล็งยิงตรงไปหาตำแหน่งของ Player เป๊ะๆ ---
            Vector2 shootDir;
            if (player != null)
            {
                // คำนวณทิศทางจากปากกระบอกปืน พุ่งไปหาตัว Player
                shootDir = (player.position - firePoint.position).normalized;
            }
            else
            {
                shootDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            }

            proj.Setup(shootDir);
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                // --- ชนปุ๊บ ตายทันที โดยการหักเลือดเท่ากับเลือดสูงสุด ---
                playerHealth.TakeDamage(playerHealth.maxHearts);
            }
        }
    }
}
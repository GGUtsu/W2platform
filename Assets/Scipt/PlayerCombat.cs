using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Tooltip("ดีเลย์ระหว่างนัด (วินาที) ค่ายิ่งน้อยยิ่งรัว เช่น 0.15 คือยิงรัวมาก")]
    public float fireRate = 0.15f;
    private float nextFireTime = 0f;

    [Tooltip("ติ๊กถูกถ้าอยากให้กดเมาส์ค้างแล้วปืนยิงรัวอัตโนมัติ")]
    public bool isAutomaticWeapon = false;

    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // เช็คว่าผู้เล่นตั้งปืนเป็นโหมดออโต้ (กดค้าง) หรือเซมิออโต้ (คลิกทีละนัด)
        bool tryToShoot = isAutomaticWeapon ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (tryToShoot && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    // ทำเป็น Public เพื่อให้ปุ่ม UI บนมือถือเรียกใช้ฟังก์ชันนี้ได้ด้วย
    public void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile proj = bullet.GetComponent<Projectile>();

            // เช็คทิศทางจาก PlayerController
            Vector2 shootDir = playerController.facingRight ? Vector2.right : Vector2.left;
            proj.Setup(shootDir);
        }
    }
}
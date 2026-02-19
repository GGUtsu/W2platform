using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // TODO: อาจจะเพิ่มแอนิเมชันกระตุก หรือเปลี่ยนสีตรงนี้

        if (currentHealth <= 0)
        {
            // ตาย
            Destroy(gameObject);
        }
    }
}
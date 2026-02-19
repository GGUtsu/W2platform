using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Image[] hearts; // ลาก Image ของหัวใจทั้ง 3 ดวงมาใส่ตรงนี้
    public Sprite fullHeart;  // รูปหัวใจเต็ม
    public Sprite emptyHeart; // รูปหัวใจว่างเปล่า

    // ฟังก์ชันนี้จะถูกเรียกเมื่อ Event OnHealthChanged ทำงาน
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart; // หัวใจที่ยังมีอยู่
            }
            else
            {
                hearts[i].sprite = emptyHeart; // หัวใจที่เสียไปแล้ว
            }
        }
    }
}
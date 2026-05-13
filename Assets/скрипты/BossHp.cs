using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 500;
    private float currentHealth;
    
    // Новая переменная для неуязвимости
    public bool isInvulnerable = false; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Если босс неуязвим — просто выходим из метода
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log("Здоровье босса: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public GameObject redLaserSquare;

    void Die()
    {
        GameObject arena = GameObject.FindWithTag("BossArena");
        if (arena != null) Destroy(arena);
        Debug.Log("Босс повержен!");
        GetComponent<Animator>().SetTrigger("Die");
        
        if (redLaserSquare != null) redLaserSquare.SetActive(false);

        Destroy(gameObject, 3f);
    }
}
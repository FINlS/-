using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 500;
    public float currentHealth;
    
    // Новая переменная для неуязвимости
    public bool isInvulnerable = false; 

    [Header("Спавн лута при смерти")]
    public GameObject keyPrefab; // Сюда перетащи префаб ключа из папки Project

    public GameObject redLaserSquare;
    public bool die = false;
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

    void Die()
    {
        if (die) return;
        die = true;
        GameObject arena = GameObject.FindWithTag("BossArena");
        if (arena != null) Destroy(arena);
        Debug.Log("Босс повержен!");
        
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Die");
        
        if (redLaserSquare != null) redLaserSquare.SetActive(false);

        // СПАВН КЛЮЧА НА МЕСТЕ БОССА
        SpawnKey();

        Destroy(gameObject, 3f);
    }

    void SpawnKey()
    {
        if (keyPrefab != null)
        {
            // Позиция спавна — координаты босса. 
            // Приподнимаем ключ на 0.5f по оси Y, чтобы он не провалился в текстуры пола.
            Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;

            // Создаем ключ на сцене со стандартным вращением (Quaternion.identity)
            Instantiate(keyPrefab, spawnPosition, Quaternion.Euler(0,0,0));
            
            Debug.Log("[Босс] Ключ успешно выпал после смерти!");
        }
        else
        {
            Debug.LogWarning("[BossHealth] Забыл назначить Key Prefab в инспекторе босса!");
        }
    }
}
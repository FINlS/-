using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [Header("Настройки падающего камня")]
    public GameObject rockPrefab;       // Префаб физического камня
    public float delayBeforeDrop = 1.5f; // Сколько секунд висит красная зона до падения
    public float spawnHeight = 12f;     // Высота, с которой полетит камень
    public float damageRadius = 2.0f;    // Радиус взрыва/урона камня
    public int damageAmount = 20;       // Сколько урона наносит

    private float timer;
    private bool rockDropped = false;

    void Start()
    {
        timer = delayBeforeDrop;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && !rockDropped)
        {
            DropRock();
        }
    }

    void DropRock()
    {
        rockDropped = true;

        // Позиция спавна камня — строго над центром красной зоны
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + spawnHeight, transform.position.z);
        
        // Создаем камень
        if (rockPrefab != null)
        {
            GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
            
            // Заставляем камень нанести урон, когда он долетит (или прямо сейчас, если это "взрывной" урон по зоне)
            // Давай сделаем честный урон по площади в момент падения:
            Invoke("DealDamage", 0.3f); // Примерное время подлета (можно настроить)
            
            Destroy(rock, 3f); // Уничтожаем камень, чтобы не лагало
        }

        // Удаляем саму красную зону
        Destroy(gameObject, 0.4f);
    }

    void DealDamage()
    {
        // Ищем всех, кто попал в зону поражения
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Предполагаем, что у игрока есть скрипт здоровья (PlayerStats или PlayerHealth)
                // Сделай под свою систему, например:
                hitCollider.GetComponent<Health>().TakeDamage(damageAmount);
                Debug.Log($"Игрок получил {damageAmount} урона от камня!");
            }
        }
    }

    // Рисуем радиус урона в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
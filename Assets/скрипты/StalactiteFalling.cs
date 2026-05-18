using UnityEngine;

public class StalactiteFalling : MonoBehaviour
{
    [Header("Красная Зона")]
    public GameObject redZonePrefab;
    public float timeBeforeFall = 1.5f;
    public float fallSpeed = 30f; // Скорость падения

    [Header("Настройки Урона")]
    public int damageAmount = 25;       // Сколько урона наносить игроку
    public float damageRadius = 2.5f;   // Радиус поражения (настрой под размер красного круга)

    private GameObject currentZone;
    private Vector3 targetGroundPos;
    private bool isFalling = false;
    private bool hasLanded = false; // Защита от повторного срабатывания

    void Start()
    {
        // 1. Пускаем луч строго вниз, чтобы найти точку на земле
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
        {
            // Запоминаем точную финальную точку падения
            targetGroundPos = hit.point;

            // Спавним зону на земле
            Vector3 zonePos = hit.point + new Vector3(0, 0.05f, 0);
            if (redZonePrefab != null)
            {
                currentZone = Instantiate(redZonePrefab, zonePos, Quaternion.identity);
            }

            // Включаем таймер до начала падения
            Invoke("StartFalling", timeBeforeFall);
        }
        else
        {
            // Если земли под нами нет — сразу удаляем
            Destroy(gameObject);
        }
    }

    void StartFalling()
    {
        isFalling = true;
    }

    void Update()
    {
        if (!isFalling || hasLanded) return;

        // Плавно перемещаем сталактит вниз к запомненной точке земли
        transform.position = Vector3.MoveTowards(transform.position, targetGroundPos, fallSpeed * Time.deltaTime);

        // Как только сталактит долетел до финальной точки
        if (Vector3.Distance(transform.position, targetGroundPos) < 0.1f)
        {
            hasLanded = true; // Блокируем повторные вызовы в следующих кадрах

            // Наносим урон игроку
            DealDamageInRadius();

            // Безопасно удаляем зону (только если она существует)
            if (currentZone != null)
            {
                Destroy(currentZone);
            }

            // Здесь можно спавнить эффект удара/пыли

            // Удаляем сам сталактит
            Destroy(gameObject);
        }
    }

    void DealDamageInRadius()
    {
        // Создаем невидимую сферу в точке приземления и находим все коллайдеры внутри неё
        Collider[] hitColliders = Physics.OverlapSphere(targetGroundPos, damageRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Ищем скрипт Health на объектах, попавших в радиус
            Health playerHealth = hitCollider.GetComponent<Health>();

            if (playerHealth != null)
            {
                // Вызываем метод нанесения урона у игрока.
                // ВАЖНО: Если в консоли появится ошибка про 'TakeDamage', 
                // значит в твоем скрипте Health этот метод называется по-другому (например, ApplyDamage).
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    // Визуальное отображение радиуса урона в окне Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetGroundPos, damageRadius);
    }
}
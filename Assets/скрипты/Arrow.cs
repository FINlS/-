using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    [Header("Настройки полета")]
    public float speed = 20f;       // Скорость полета стрелы
    public int damage = 15;         // Урон, который получит игрок
    public float lifeTime = 5f;     // Через сколько секунд стрела удалится, если никуда не попала

    private Rigidbody rb;
    private bool hasHit = false;    // Защита от двойного срабатывания урона

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Выключаем гравитацию, чтобы стрела летела ровно без просадки
            rb.useGravity = false; 

            // Запускаем стрелу вперед по направлению ее синей оси (Z / forward)
            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError($"[Стрела] На объекте {gameObject.name} не найден компонент Rigidbody!");
        }

        // Автоматически удаляем стрелу через время lifeTime, чтобы избежать лагов
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Если стрела уже во что-то попала, игнорируем дальнейшие столкновения
        if (hasHit) return;

        // Проверяем, попали ли мы в игрока
        if (other.CompareTag("Player"))
        {
            // Ищем скрипт здоровья на объекте, в который врезались
            Health playerHealth = other.GetComponent<Health>();

            // Безопасный поиск: если коллайдер на кости скелета, ищем в родителе
            if (playerHealth == null)
            {
                playerHealth = other.GetComponentInParent<Health>();
            }

            // Наносим урон только если скрипт здоровья реально существует
            if (playerHealth != null)
            {
                hasHit = true;
                Debug.Log($"[Попадание] Стрела врага нанесла {damage} урона Игроку!");
                
                playerHealth.TakeDamage(damage);

                // Удаляем стрелу сразу после нанесения урона
                Destroy(gameObject);
            }
        }
        // Проверяем попадание в препятствие (земля, стены, укрытия)
        else if (other.CompareTag("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasHit = true;
            Debug.Log("[Попадание] Стрела вонзилась в землю/препятствие.");

            // Останавливаем физику стрелы, чтобы она красиво осталась в стене
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Удаляем стрелу чуть позже, через 1.5 секунды, для красоты
            Destroy(gameObject, 1.5f);
        }
    }
}
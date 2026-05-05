using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3f;
    private Transform player;
    private Rigidbody rb;
    public float damage = 10f;
    public float attackSpeed = 1f; // Раз в секунду
    private float nextAttackTime;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            nextAttackTime = Time.time + attackSpeed;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Находим игрока по тегу (убедись, что у игрока стоит тег "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;
        Health health = GetComponent<Health>();

    // 2. Если здоровья нет или персонаж мертв — выходим из Update
        if (health != null && health.currentHealth <= 0) 
        {
            return; // Код ниже (движение и анимация бега) не выполнится
        }
        // Вектор направления к игроку
        Vector3 direction = (player.position - transform.position).normalized;
        // Убираем движение по вертикали (Y), чтобы враги не взлетали
        direction.y = 0;

        // Двигаем врага через Rigidbody
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
        
        // Поворачиваем врага в сторону игрока (опционально)
        if (direction != Vector3.zero)
            transform.forward = direction;
    }
}
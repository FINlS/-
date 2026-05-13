using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 25f;
    public float lifetime = 3f; // Чтобы пуля не летела вечно

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Если попали во врага
        if (other.CompareTag("Enemy")||other.CompareTag("Boss"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            BossHealth health = other.GetComponent<BossHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Пуля исчезает при попадании
        }
    }
}
using UnityEngine;

public class BossAOE : MonoBehaviour
{
    public float damageRadius = 5f;
    public int damageAmount = 30;
    public LayerMask playerLayer; // Выбери слой "Player"
    public GameObject impactEffect; // Префаб частиц (пыль, взрыв)

    public void Explode()
    {
        // Эффект частиц
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Поиск игрока в радиусе
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, playerLayer);

        foreach (var hitCollider in hitColliders)
        {
            // Наносим урон через твой скрипт здоровья
            var health = hitCollider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                Debug.Log("Игрок задет прыжком!");
            }
        }
    }

    // Рисуем радиус в редакторе для настройки
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
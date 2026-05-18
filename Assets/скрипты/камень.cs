using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [HideInInspector] public int damage;
    [HideInInspector] public float radius;

    private bool hasExploded = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Чтобы камень не взрывался дважды (например, задев сначала стену, а потом пол)
        if (hasExploded) return;
        hasExploded = true;

        Explode();
    }

    void Explode()
    {
        Debug.Log("[Камень] Бум! Камень упал на землю.");

        // Ищем всех в радиусе поражения в момент удара
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Наносим урон игроку (замени на свою систему здоровья, если она есть)
                // hitCollider.GetComponent<PlayerHealth>().TakeDamage(damage);
                Debug.Log($"[УРОН] Игрок получил {damage} урона от упавшего камня!");
            }
        }

        // Тут можно спавнить эффект пыли или взрыва, если есть Particle System
        
        // Удаляем камень со сцены сразу после удара
        Destroy(gameObject);
    }

    // Отрисовка радиуса взрыва в редакторе (в режиме игры при выделении объекта)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
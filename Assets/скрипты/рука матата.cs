using UnityEngine;

public class BossHand : MonoBehaviour
{
    public float damage = 60f;
    private Collider handCollider;

    void Start()
    {
        handCollider = GetComponent<Collider>();
        handCollider.enabled = false; // В начале рука не наносит урон
    }

    // Эти методы мы будем вызывать из анимации
    public void EnableHandCollider() => handCollider.enabled = true;
    public void DisableHandCollider() => handCollider.enabled = false;

    void OnTriggerEnter(Collider other)
    {
        // Проверяем, что коснулись игрока
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().TakeDamage(damage);
            Debug.Log("Босс ударил игрока!");
            
            DisableHandCollider();
        }
    }
}
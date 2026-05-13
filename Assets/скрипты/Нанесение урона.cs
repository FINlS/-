using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 20;
    public float attackSpeed = 1f; 
    private float nextAttackTime;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
            nextAttackTime = Time.time + attackSpeed;
        }
    }
}
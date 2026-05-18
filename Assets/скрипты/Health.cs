using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Экран Смерти")]
    public GameOverManager gameOverManager;
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject expGemPrefab;
    public bool isPlayer = false;
    public Slider healthBar;

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>(); // Ищем аниматор в модели
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Если уже мертв, урон не принимаем

        currentHealth -= damage;
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (isPlayer && healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // МЕНЯЕМ ТЕГ, чтобы пули и наводка его не видели
        gameObject.tag = "Untagged"; 
        // Это полностью запретит скриптам менять параметры аниматора после смерти
        // Если хочешь, чтобы анимация смерти доиграла, используй это вместо отключения:
        anim.SetBool("isDead", true); // Создай такой Bool параметр в Animator


        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
            
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        if (expGemPrefab != null) Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 3f);
        if (gameOverManager != null && isPlayer)
        {
            gameOverManager.ShowGameOverScreen();
        }
    }
}
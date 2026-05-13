using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform player;
    public BossHand bossHand; 
    public GameObject jumpIndicator; // Красный круг (предупреждение)
    private Animator anim;
    private BossAOE aoe;
    private BossHealth health;

    [Header("Движение")]
    public float speed = 3f;
    public float rotationSpeed = 5f;

    [Header("Дистанции")]
    public float stopDistance = 2.5f;   
    public float jumpDistanceMin = 7f;  
    public float jumpDistanceMax = 12f; 

    [Header("Тайминги")]
    public float attackCooldown = 2f;   
    public float jumpCooldown = 3f; // Бег между прыжками минимум 3 сек
    public float startDelay = 2f;   // Время АФК при спавне

    private bool isAttacking = false;
    private float nextJumpAllowTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        aoe = GetComponent<BossAOE>();
        health = GetComponent<BossHealth>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (jumpIndicator != null) jumpIndicator.SetActive(false);

        // ЗАПУСК НАЧАЛЬНОЙ ПАУЗЫ
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        isAttacking = true; // Блокируем Update
        anim.SetFloat("Speed", 0f);

        // Включаем неуязвимость в скрипте здоровья
        if (health != null) health.isInvulnerable = true;

        Debug.Log("Босс пробуждается: АФК и Неуязвим...");
        yield return new WaitForSeconds(startDelay);

        // Выключаем неуязвимость и даем боссу начать бой
        if (health != null) health.isInvulnerable = false;
        isAttacking = false;
        Debug.Log("Босс вступил в бой!");
    }

    void Update()
    {
        // Если босс атакует, прыгает или еще в начале АФК — не двигаемся
        if (player == null || isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Поворот к игроку
        RotateTowardsTarget(player.position, rotationSpeed);

        if (distance > stopDistance)
        {
            MoveTowardsTarget();

            // Логика прыжка
            if (distance >= jumpDistanceMin && distance <= jumpDistanceMax)
            {
                if (Time.time >= nextJumpAllowTime)
                {
                    StartCoroutine(PerformJump());
                }
            }
        }
        else
        {
            // Удар рукой
            StartCoroutine(PerformPunch());
        }
    }

    void MoveTowardsTarget()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        anim.SetFloat("Speed", 1f);
    }

    void RotateTowardsTarget(Vector3 targetPos, float rotSpeed)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0; 
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
        }
    }

    IEnumerator PerformPunch()
    {
        isAttacking = true;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Punch");
        
        // Ожидание отката атаки
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator PerformJump()
    {
        isAttacking = true;
        anim.SetFloat("Speed", 0f);

        // 1. ПРИЦЕЛИВАНИЕ
        float aimTimer = 0;
        while (aimTimer < 0.5f)
        {
            RotateTowardsTarget(player.position, rotationSpeed * 2f);
            aimTimer += Time.deltaTime;
            yield return null;
        }

        Vector3 targetPoint = player.position;
        targetPoint.y = transform.position.y; 

        // 2. ИНДИКАТОР (фиксируем на земле)
        if (jumpIndicator != null)
        {
            jumpIndicator.SetActive(true);
            jumpIndicator.transform.position = targetPoint + Vector3.up * 0.05f;
            jumpIndicator.transform.SetParent(null); // Отвязываем от босса
        }

        anim.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(0.3f); 

        // 3. ПОЛЕТ
        float jumpSpeed = speed * 4f;
        while (Vector3.Distance(transform.position, targetPoint) > 0.3f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, jumpSpeed * Time.deltaTime);
            yield return null;
        }

        // 4. ПРИЗЕМЛЕНИЕ
        if (jumpIndicator != null)
        {
            jumpIndicator.SetActive(false);
            jumpIndicator.transform.SetParent(this.transform); // Возвращаем в иерархию босса
        }

        if (aoe != null) aoe.Explode();
        
        nextJumpAllowTime = Time.time + jumpCooldown; // КД на следующий прыжок
        yield return new WaitForSeconds(0.4f); 

        isAttacking = false; // Возвращаемся к бегу
        anim.SetFloat("Speed", 1f); 
    }

    // Методы для Animation Events
    public void EnableHandCollider() { if(bossHand != null) bossHand.EnableHandCollider(); }
    public void DisableHandCollider() { if(bossHand != null) bossHand.DisableHandCollider(); }
}
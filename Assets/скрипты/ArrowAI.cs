using UnityEngine;
using System.Collections;

public class ArrowAI : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform player;
    public GameObject redZoneIndicator; // Красный круг под ногами игрока при прицеливании
    public GameObject arrowPrefab;      // Префаб стрелы (на котором скрипт EnemyArrow)
    public Transform firePoint;         // Пустышка на луке (откуда вылетит стрела)
    private Animator anim;

    [Header("Движение")]
    public float speed = 3.5f;
    public float rotationSpeed = 12f;

    [Header("Дистанции")]
    public float meleeRange = 1.3f;       // Расстояние для удара ближнего боя
    public float attackRange = 8f;        // Расстояние для стрельбы из лука
    public float visionRange = 15f;       // Дистанция, на которой замечает игрока

    [Header("Тайминги")]
    public float fireRate = 2f;           // Перезарядка стрельбы
    public float meleeRate = 1.5f;        // Перезарядка удара руками
    public float startDelay = 1f;         // Задержка при спавне (АФК)

    private bool isAttacking = false;
    private float nextAttackAllowTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (redZoneIndicator != null) redZoneIndicator.SetActive(false);

        // Начальная пауза при спавне
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        isAttacking = true; 
        if (anim != null) anim.SetFloat("Speed", 0f);

        yield return new WaitForSeconds(startDelay);

        isAttacking = false;
    }

    void Update()
    {
        Health health = GetComponent<Health>();

        if (player == null || isAttacking || health.currentHealth <= 0) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Если игрок вне зоны видимости — стоим на месте
        if (distance > visionRange)
        {
            StopMoving();
            return;
        }

        // Поворот к игроку
        RotateTowardsTarget(player.position, rotationSpeed);

        // ПРОВЕРКА ДИСТАНЦИЙ АТАКИ И ПРЕСЛЕДОВАНИЯ
        if (distance <= meleeRange)
        {
            // 1. БЛИЖНИЙ БОЙ
            if (Time.time >= nextAttackAllowTime)
            {
                StartCoroutine(PerformMeleeAttack());
            }
            else
            {
                StopMoving();
            }
        }
        else if (distance <= attackRange)
        {
            // 2. ДАЛЬНИЙ БОЙ (СТРЕЛЬБА)
            if (Time.time >= nextAttackAllowTime)
            {
                StartCoroutine(PerformRangedAttack());
            }
            else
            {
                StopMoving();
            }
        }
        else
        {
            // 3. ПРЕСЛЕДОВАНИЕ (Игрок далеко, но в зоне видимости)
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (anim != null) anim.SetFloat("Speed", speed);
    }

    void StopMoving()
    {
        if (anim != null) anim.SetFloat("Speed", 0f);
    }

    void RotateTowardsTarget(Vector3 targetPos, float rotSpeed)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0; 
        
        if (direction != Vector3.zero)
        {
            // 1. Считаем базовый поворот строго на игрока
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            
            // 2. Создаем дополнительное смещение на -10 градусов по оси Y (влево)
            Quaternion leftOffset = Quaternion.Euler(0, -8f, 0);
            
            // 3. Умножаем базовый поворот на смещение, чтобы получить итоговую цель
            Quaternion finalRotation = lookRotation * leftOffset;
            
            // Плавный разворот к новой скорректированной цели
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * rotSpeed);
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;
        StopMoving();

        if (anim != null) anim.SetTrigger("MeleeAttack");
        
        PerformMeleeDamage();

        nextAttackAllowTime = Time.time + meleeRate;
        yield return new WaitForSeconds(0.5f); // Время на доигровку анимации

        isAttacking = false;
    }

    IEnumerator PerformRangedAttack()
    {
        isAttacking = true;
        StopMoving();

        // 1. ПРИЦЕЛИВАНИЕ С КРАСНОЙ ЗОНОЙ
        if (redZoneIndicator != null)
        {
            redZoneIndicator.SetActive(true);
            Vector3 targetPoint = player.position;
            targetPoint.y = transform.position.y; 
            redZoneIndicator.transform.position = targetPoint + Vector3.up * 0.05f;
            redZoneIndicator.transform.SetParent(null); // Отвязываем от лучника
        }

        float aimTimer = 0;
        while (aimTimer < 0.5f)
        {
            RotateTowardsTarget(player.position, rotationSpeed * 1.5f);
            aimTimer += Time.deltaTime;
            yield return null;
        }

        if (anim != null) anim.SetTrigger("Shoot");

        // 2. ВЫСТРЕЛ СИЛАМИ ЛУЧНИКА
        if (arrowPrefab != null && firePoint != null)
        {
            Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            Debug.Log("[ArrowAI] Стрела создана и выпущена!");
        }
        else
        {
            Debug.LogError("[ArrowAI] Не назначен Префаб стрелы или Точка выстрела (FirePoint)!");
        }

        // Убираем индикатор обратно
        if (redZoneIndicator != null)
        {
            redZoneIndicator.SetActive(false);
            redZoneIndicator.transform.SetParent(this.transform); 
        }

        nextAttackAllowTime = Time.time + fireRate;
        yield return new WaitForSeconds(0.4f); // Время на завершение анимации

        isAttacking = false;
    }

    void PerformMeleeDamage()
    {
        Vector3 attackCenter = transform.position + transform.forward * 0.5f;
        Collider[] hitColliders = Physics.OverlapSphere(attackCenter, 1.0f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Health playerHealth = hitCollider.GetComponent<Health>();
                if (playerHealth == null) playerHealth = hitCollider.GetComponentInParent<Health>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(20); // Наносим 20 урона вблизи
                    Debug.Log("[ArrowAI] Успешный удар руками по игроку!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
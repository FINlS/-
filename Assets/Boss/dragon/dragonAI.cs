using UnityEngine;
using System.Collections;

public class BossAI1 : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public Transform playe;
    public BossHand bossHand;          // Скрипт меча / руки (включает коллайдер оружия)
    public Collider tailCollider;      // Коллайдер хвоста (Is Trigger обязательно!)
    public GameObject laserVFXObject;  // Готовый объект лазера внутри анимированной кости
    public Collider laserCollider;      // Коллайдер лазера для урона луча

    private Animator anim;
    private BossHealth health;

    [Header("Движение")]
    public float speed = 3f;
    public float rotationSpeed = 5f;

    [Header("Дистанции для атак")]
    public float swordDistance = 4f;     // Дистанция удара мечом (вплотную)
    public float tailDistanceMax = 6f;   // Дистанция удара хвостом (средняя)
    public float laserDistanceMin = 12f; // Дистанция для лазера (игрок очень далеко)

    [Header("Тайминги и Откаты")]
    public float attackCooldown = 1f;    // Общий КД между физическими ударами (меч/хвост)
    public float laserCooldown = 6f;     // КД конкретно на лазер
    public float tailCooldown = 6f;      // КД конкретно на хвост
    public float laserDuration = 5f;     // Время горения лазера
    public float startDelay = 2f;        // Время АФК при спавне босса

    private bool isAttacking = false;
    private float nextLaserAllowTime = 0f;
    private float nextTailAllowTime = 0f;
    private bool LaserDamaged = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<BossHealth>();
        
        // Автоматический поиск игрока по тегу
        if (playe == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playe = playerObj.transform;
        }

        // Выключаем лазер, его коллайдер и коллайдер хвоста на старте игры
        if (laserVFXObject != null) laserVFXObject.SetActive(false);
        if (laserCollider != null) laserCollider.enabled = false;
        if (tailCollider != null) tailCollider.enabled = false;

        // ЗАПУСК НАЧАЛЬНОЙ ПАУЗЫ (Пробуждение)
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        isAttacking = true; // Блокируем Update на время пробуждения
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
        // Если нет игрока, босс уже атакует, или босс мертв — ничего не делаем
        if (playe == null || isAttacking || health.currentHealth <= 0) return;

        float distance = Vector3.Distance(transform.position, playe.position);

        // Поворот к игроку в фазе преследования/бега
        RotateTowardsTarget(playe.position, rotationSpeed);

        // ДЕРЕВО ПОВЕДЕНИЯ БОССА
        if (distance <= swordDistance)
        {
            // 1. Игрок вплотную — бьем мечом
            StartCoroutine(PerformSwordAttack());
        }
        else if (distance > swordDistance && distance <= tailDistanceMax && Time.time >= nextTailAllowTime)
        {
            // 2. Игрок на средней дистанции — бьем хвостом
            StartCoroutine(PerformTailAttack());
        }
        else if (distance >= laserDistanceMin && Time.time >= nextLaserAllowTime)
        {
            // 3. Игрок далеко и лазер перезарядился — включаем лазер
            LaserDamaged = false;
            StartCoroutine(PerformLaserAttack());
        }
        else
        {
            // 4. Во всех остальных случаях — просто бежим к нему
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        // Движение с учетом инверсии на 180 градусов
        transform.Translate(Vector3.forward * -speed * Time.deltaTime);
        anim.SetFloat("Speed", 1f); // Включаем анимацию бега
    }

    void RotateTowardsTarget(Vector3 targetPos, float rotSpeed)
    {
        // Направление с учетом инверсии на 180 градусов
        Vector3 direction = (-targetPos + transform.position).normalized;
        direction.y = 0; // Чтобы босс не заваливался вверх/вниз
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
        }
    }

    // --- БЛОК АТАК (КОРУТИНЫ) ---

    // УДАР МЕЧОМ
    IEnumerator PerformSwordAttack()
    {
        isAttacking = true;
        anim.SetFloat("Speed", 0f); 
        anim.SetTrigger("SwordAttack");
        
        // Включаем коллайдер меча в начале корутины (если не используешь Animation Events)
        EnableHandCollider();
        
        yield return new WaitForSeconds(attackCooldown);
        
        // Выключаем после завершения атаки
        DisableHandCollider();
        isAttacking = false;
    }

    // УДАР ХВОСТОМ
    IEnumerator PerformTailAttack()
    {
        isAttacking = true;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("TailAttack");
        
        // Включаем коллайдер хвоста
        EnableTailCollider();
        
        yield return new WaitForSeconds(attackCooldown);
        
        // Выключаем коллайдер хвоста
        DisableTailCollider();
        isAttacking = false;
        nextTailAllowTime = Time.time + tailCooldown; // Ставим КД на хвост
    }

    // АТАКА ЛАЗЕРОМ (Включение готового VFX на кости)
    IEnumerator PerformLaserAttack()
    {
        isAttacking = true;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("LaserCast");
        
        // Ожидание (задержка каста), чтобы босс успел встать в позу перед лучом
        yield return new WaitForSeconds(0.4f); 

        // Включаем сам объект лазера и его физический коллайдер урона
        if (laserVFXObject != null) laserVFXObject.SetActive(true);
        if (laserCollider != null) laserCollider.enabled = true;

        // Ждем ровно столько, сколько лазер должен гореть по задумке
        yield return new WaitForSeconds(laserDuration);

        // Выключаем лазер и коллайдер обратно в спящий режим
        if (laserVFXObject != null) laserVFXObject.SetActive(false);
        if (laserCollider != null) laserCollider.enabled = false;
        
        // Ставим лазер на перезарядку
        nextLaserAllowTime = Time.time + laserCooldown; 
        
        yield return new WaitForSeconds(0.5f); // Пауза после каста
        isAttacking = false;
    }

    // --- БЛОК ФИЗИКИ И УРОНА ---

    // Выжигание здоровья лазером (работает каждый кадр, пока игрок внутри)
    private void OnTriggerStay(Collider other)
    {
        if (laserCollider != null && laserCollider.enabled && other.CompareTag("Player"))
        {
            Debug.Log("Лазер выжигает здоровье игрока каждую секунду...");
            
            // Ищем скрипт здоровья на игроке (убедись, что он называется Health или поменяй название)
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(200f * Time.deltaTime);
            }
        }
    }

    // Разовый удар хвостом (срабатывает в момент касания)
    private void OnTriggerEnter(Collider other)
    {
        if (tailCollider != null && tailCollider.enabled && other.CompareTag("Player"))
        {
            // Пробуем получить скрипт управления с игрока
            управление playerControl = other.GetComponent<управление>();

            // Если скрипт найден и игрок НЕ стоит на земле (значит, он в прыжке)
            if (playerControl != null && !playerControl.isGrounded)
            {
                Debug.Log("Игрок подпрыгнул! Удар хвостом пролетел снизу.");
                return; // Выходим из метода, урон отменяется
            }

            // Если игрок на земле (или скрипт управления почему-то не нашелся) — наносим урон
            Debug.Log("Босс попал по игроку ХВОСТОМ!");
            
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(30);
            }
        }
    }

    // --- МЕТОДЫ ДЛЯ ANIMATION EVENTS МЕЧА ---
    public void EnableHandCollider() { if (bossHand != null) bossHand.EnableHandCollider(); }
    public void DisableHandCollider() { if (bossHand != null) bossHand.DisableHandCollider(); }

    // --- МЕТОДЫ ДЛЯ ANIMATION EVENTS ХВОСТА ---
    public void EnableTailCollider() { if (tailCollider != null) tailCollider.enabled = true; }
    public void DisableTailCollider() { if (tailCollider != null) tailCollider.enabled = false; }
}
using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform playe;
    public BossHand bossHand; 
    public GameObject jumpIndicator; // Красный круг (предупреждение)
    private Animator anim;
    private BossAOE aoe;
    private BossHealth health;

    [Header("Аудио")]
    public AudioSource bossAudioSource; 
    public AudioClip wakeUpSound;       
    public AudioClip punchSound;        
    public AudioClip jumpStartSound;    
    public AudioClip landSound;         

    public float punchSoundOffset = 0.0f;
    public float landSoundOffset = 0.0f; // НОВАЯ СТРОКА: отступ для приземления (в секундах)

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
        
        // Автоматически ищем AudioSource на боссе, если забыли перетащить в инспекторе
        if (bossAudioSource == null)
        {
            bossAudioSource = GetComponent<AudioSource>();
        }

        if (playe == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playe = playerObj.transform;
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
        
        // ЗВУК: Воспроизводим рык пробуждения
        PlayBossSound(wakeUpSound, 1f);

        yield return new WaitForSeconds(startDelay);

        // Выключаем неуязвимость и даем боссу начать бой
        if (health != null) health.isInvulnerable = false;
        isAttacking = false;
        Debug.Log("Босс вступил в бой!");
    }

    void Update()
    {
        // Если босс атакует, прыгает или еще в начале АФК — не двигаемся
        if (playe == null || isAttacking || health.currentHealth <= 0) return;

        float distance = Vector3.Distance(transform.position, playe.position);

        // Поворот к игроку
        RotateTowardsTarget(playe.position, rotationSpeed);

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
        anim.SetTrigger("Punch"); // Босс начал анимацию удара
    
        // Ждем ровно 1 секунду, пока босс замахивается (рука летит к игроку)
        yield return new WaitForSeconds(1f);

        // --- ЗАПУСК ЗВУКА ЧЕРЕЗ СЕКУНДУ ПОСЛЕ НАЧАЛА УДАРА ---
        if (bossAudioSource != null && punchSound != null)
        {
            bossAudioSource.clip = punchSound;
            bossAudioSource.volume = 0.8f;
            bossAudioSource.Play();
        
            // По-прежнему проматываем стартовую тишину самого аудиофайла, если она там есть
            bossAudioSource.time = punchSoundOffset; 
        }
        // ----------------------------------------------------
    
        // Ожидание полного отката атаки перед тем, как босс снова сможет ходить
        // Если attackCooldown меньше 1 секунды, можно поставить здесь фиксированное время, например 0.5f
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
            RotateTowardsTarget(playe.position, rotationSpeed * 2f);
            aimTimer += Time.deltaTime;
            yield return null;
        }

        Vector3 targetPoint = playe.position;
        targetPoint.y = transform.position.y; 

        // 2. ИНДИКАТОР (фиксируем на земле)
        if (jumpIndicator != null)
        {
            jumpIndicator.SetActive(true);
            jumpIndicator.transform.position = targetPoint + Vector3.up * 0.05f;
            jumpIndicator.transform.SetParent(null); 
        }

        anim.SetTrigger("JumpAttack");
    
        // Звук подготовки к прыжку (играет нормально через PlayOneShot)
        PlayBossSound(jumpStartSound, 0.9f);

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
            jumpIndicator.transform.SetParent(this.transform); 
        }

        if (aoe != null) aoe.Explode();
    
        // --- ОСОБЫЙ ЗАПУСК ЗВУКА ПРИЗЕМЛЕНИЯ БЕЗ ТИШИНЫ ---
        if (bossAudioSource != null && landSound != null)
        {
            bossAudioSource.clip = landSound;
            bossAudioSource.volume = 1f;
            bossAudioSource.Play();
        
            // Мгновенно проматываем тишину на приземлении!
            bossAudioSource.time = landSoundOffset; 
        }
        // --------------------------------------------------
    
        nextJumpAllowTime = Time.time + jumpCooldown; 
        yield return new WaitForSeconds(0.4f); 

        isAttacking = false; 
        anim.SetFloat("Speed", 1f); 
    }
    // Универсальный внутренний метод для безопасного проигрывания звуков
    private void PlayBossSound(AudioClip clip, float volume)
    {
        if (bossAudioSource != null && clip != null)
        {
            bossAudioSource.PlayOneShot(clip, volume);
        }
    }

    // Методы для Animation Events
    public void EnableHandCollider() { if(bossHand != null) bossHand.EnableHandCollider(); }
    public void DisableHandCollider() { if(bossHand != null) bossHand.DisableHandCollider(); }
}
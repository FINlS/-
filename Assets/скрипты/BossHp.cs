using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 500;
    public float currentHealth;
    
    // Новая переменная для неуязвимости
    public bool isInvulnerable = false; 

    [Header("Спавн лута при смерти")]
    public GameObject keyPrefab; // Сюда перетащи префаб ключа из папки Project

    [Header("Ссылки на объекты")]
    public GameObject redLaserSquare;
    public bool die = false;
    public VictoryManager victoryManager;
    [Header("Настройки быстрого побега")]
    public float escapeSpeed = 10f;       // Скорость, с которой босс будет убегать
    public Transform escapeTarget;        // (Опционально) Точка, КУДА босс побежит. Если пусто — побежит просто ОТ игрока.

    private bool isEscaping = false;      // Флаг, что босс сейчас в режиме побега
    private Vector3 escapeDirection;      // Направление бега

    public enum AfterDeathAction { LoadNextScene, ShowVictoryScreen }

    [Header("Что делать после побега босса?")]
    public AfterDeathAction whatToDo; 

    [Header("Если нужно загрузить следующую сцену:")]
    public string nextSceneName; // Имя сцены (например, Level2), если выбрали LoadNextScene

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Если босс неуязвим — просто выходим из метода
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log("Здоровье босса: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (die) return;
        die = true;
        if (redLaserSquare != null) 
        {
            redLaserSquare.SetActive(false);
            Debug.Log("[Босс] Лазеры выключены.");
        }

        Debug.Log("Босс повержен и решает сбежать!");
        
        // 1. Включаем анимацию БЕГА (вместо смерти), чтобы он быстро перебирал лапами
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Die"); // Убедись, что триггер называется так же в Аниматоре
        
        
        if (redLaserSquare != null) redLaserSquare.SetActive(false);
        if (whatToDo == AfterDeathAction.ShowVictoryScreen)
        {
            // 2. Отключаем NavMeshAgent, чтобы он не мешал скрипту двигать босса сквозь любые препятствия
            var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            // 3. Переводим коллайдер в режим триггера, чтобы босс не врезался в игрока и окружение
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
            GameObject player = GameObject.FindWithTag("Player");

            // 4. Вычисляем направление бега
            if (escapeTarget != null)
            {
                // Если указана конкретная точка — бежим к ней
                escapeDirection = (player.transform.position).normalized;
            }
            else
            {
                // Если точки нет — бежим в противоположную от игрока сторону
                if (player != null)
                {
                    escapeDirection = (transform.position - player.transform.position).normalized;
                }
                else
                {
                    escapeDirection = transform.forward; // На крайний случай — просто строго вперед
                }
            }
            
            // Выравниваем направление по горизонтали, чтобы босс не взлетал в воздух и не зарывался в землю
            escapeDirection.y = 0; 

            // Разворачиваем босса лицом туда, куда он сейчас побежит
            if (escapeDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(escapeDirection);
            }

            // Включаем логику движения в Update
            isEscaping = true;
            Invoke("vm1", 3f);

        }
        // СПАВН КЛЮЧА НА МЕСТЕ, ГДЕ БОССА ДОБИЛИ
        SpawnKey();

        // Полностью удаляем босса через 3 секунды, когда он скроется из виду
        Destroy(gameObject, 3f);
    }
    void vm1()
    {
        victoryManager.TriggerVictory();
    }
    void Update()
    {
        // Если босс перешел в режим побега — каждую секунду несем его вперед с бешеной скоростью
        if (isEscaping)
        {
            transform.Translate(-Vector3.forward * escapeSpeed * Time.deltaTime);
            
            // Плавное визуальное растворение (уменьшаем масштаб до нуля, чтобы он исчезал "вдали")
        }
    }


    void SpawnKey()
    {
        if (keyPrefab != null)
        {
            // Позиция спавна — координаты босса. 
            // Приподнимаем ключ на 0.5f по оси Y, чтобы он не провалился в текстуры пола.
            Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;

            // Создаем ключ на сцене со стандартным вращением (Quaternion.identity)
            Instantiate(keyPrefab, spawnPosition, Quaternion.identity);
            
            Debug.Log("[Босс] Ключ успешно выпал после смерти!");
        }
        else
        {
            Debug.LogWarning("[BossHealth] Забыл назначить Key Prefab в инспекторе босса!");
        }
    }
}
using UnityEngine;
using TMPro; // Для работы с текстом интерфейса

public class EnemySpawner : MonoBehaviour
{
    [Header("Ссылки на объекты")]
    public GameObject enemyPrefab;    // Префаб красного кубика
    public Transform playe;          // Ссылка на трансформ игрока
    public TextMeshProUGUI timerText; // Ссылка на текстовый объект в Canvas

    [Header("Настройки спавна")]
    public float spawnRadius = 20f;         // Дистанция появления врагов
    public float initialSpawnInterval = 2f; // Начальная пауза между врагами
    public float minimumSpawnInterval = 0.3f; // Лимит скорости спавна
    public float difficultyScaling = 0.02f;  // На сколько секунд уменьшать интервал каждый кадр
    public int maxEnemies = 100;            // Максимум врагов на карте одновременно

    [Header("Статистика (только чтение)")]
    public float gameTimer = 0f;
    private float currentInterval;
    private float nextSpawnTime;

    void Start()
    {
        currentInterval = initialSpawnInterval;
        nextSpawnTime = Time.time + currentInterval;

        // Если забыл привязать игрока в инспекторе, попробуем найти его по тегу
        if (playe == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playe = p.transform;
        }
    }

    void Update()
    {
        if (playe == null) return;

        // 1. Обновляем время
        gameTimer += Time.deltaTime;
        UpdateTimerUI();

        // 2. Усложняем игру: постепенно уменьшаем интервал между врагами
        if (currentInterval > minimumSpawnInterval)
        {
            currentInterval -= difficultyScaling * Time.deltaTime;
        }

        // 3. Логика спавна по таймеру
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnEnemy();
            nextSpawnTime = Time.time + currentInterval;
        }
    }

    void TrySpawnEnemy()
    {
        // Проверяем, не слишком ли много врагов уже на сцене
        // Для этого у префаба врага ОБЯЗАТЕЛЬНО должен быть тег "Enemy"
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Генерируем случайную точку на окружности вокруг игрока
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = new Vector3(randomPoint.x, 0.5f, randomPoint.y) + playe.position;

        // Создаем врага
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTimer / 60);
            int seconds = Mathf.FloorToInt(gameTimer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
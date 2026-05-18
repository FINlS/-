using System;
using UnityEngine;

public class DynamicStalactiteSpawner : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject stalactitePrefab; // Префаб сталактита (со скриптом StalactiteBehaviour)

    [Header("Привязка к игроку")]
    public Transform playerTransform; // Сюда перетащи игрока (или найдет кодом через тег Player)

    [Header("Настройки радиуса спавна")]
    public float minSpawnRadius = 5f;  // Не ближе чем Х метров от игрока
    public float maxSpawnRadius = 25f; // Не дальше чем Y метров от игрока

    [Header("Настройки высоты")]
    public float spawnHeight = 20f;        // На какой высоте над землей создавать сталактит
    public float skyCheckHeight = 40f;    // Высота, с которой пускаем луч для поиска земли

    [Header("Тайминги")]
    public float spawnInterval = 2f; // Насколько часто падают сосульки

    private void Start()
    {
        // Если забыл привязать игрока в инспекторе, пробуем найти по тегу
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        // Запуск бесконечного цикла спавна
        InvokeRepeating("SpawnNearPlayer", 2f, spawnInterval);
    }

    void SpawnNearPlayer()
    {
        if (playerTransform == null) return;

        // 1. Получаем случайную точку на плоскости внутри круга
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);

        // ИСПРАВЛЕНО: Для targetZ теперь используется playerTransform.position.z вместо .y
        float targetX = playerTransform.position.x + randomCircle.x;
        float targetZ = playerTransform.position.z + randomCircle.y;

        // Точка высоко в небе, откуда будем искать землю
        Vector3 rayOrigin = new Vector3(targetX, skyCheckHeight, targetZ);

        // 2. Стреляем лучом вниз, чтобы убедиться, что там есть сгенерированная земля
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, skyCheckHeight + 10f))
        {
            // Земля найдена! Хит-поинт — это точное место падения.
            Vector3 groundPosition = hit.point;

            // Вычисляем позицию для спавна сталактита (строго над землей на заданной высоте)
            Vector3 spawnPosition = groundPosition + new Vector3(0, spawnHeight, 0);

            // 3. Создаем сталактит (он сразу перевернут на 180 градусов по X, то есть острием вниз)
            // ИСПРАВЛЕНО: Убран старый кусок с поиском скрипта StalactiteFalling и вызовом TriggerFall.
            // Теперь скрипт StalactiteBehaviour на самом сталактите всё сделает сам автоматически в своем Start()
            Instantiate(stalactitePrefab, spawnPosition, Quaternion.Euler(180f, 0f, 0f));
        }
    }

    // Визуализация радиуса спавна вокруг игрока в редакторе
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerTransform.position, minSpawnRadius);
        Gizmos.DrawWireSphere(playerTransform.position, maxSpawnRadius);
    }
}
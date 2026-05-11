using UnityEngine;

public class TileRandomDecorator : MonoBehaviour
{
    [Header("Настройки объектов")]
    public GameObject[] props;           // Префабы деревьев, камней
    public LayerMask propsLayer;        // Слой "Props", чтобы объекты не слипались
    
    [Header("Настройки спавна")]
    public int minObjects = 1;           // Минимальное кол-во объектов на плитке
    public int maxObjects = 5;           // Максимальное кол-во
    public float tileSize = 10f;         // Размер плитки
    public float forbiddenRadius = 2f;   // Радиус проверки коллизий
    public float groundedY = 0f;         // Точная мировая высота земли (обычно 0)

    void Start()
    {
        // Если префабы не назначены, ничего не делаем
        if (props == null || props.Length == 0) return;

        int count = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < count; i++)
        {
            TrySpawnObject();
        }
    }

    void TrySpawnObject()
    {
        // 1. Генерируем случайную позицию ВНУТРИ плитки в её локальных координатах.
        // Мы используем X и Z, предполагая, что Y - это высота.
        float halfSize = tileSize / 2f - 1.0f; // Небольшой отступ от края
        Vector3 randomLocalPos = new Vector3(
            Random.Range(-halfSize, halfSize),
            0, // В локальных координатах высота пока 0
            Random.Range(-halfSize, halfSize)
        );

        // 2. ИСПРАВЛЕНИЕ ВЫСОТЫ:
        // Сначала переводим локальную позицию в МИРОВУЮ.
        // В этот момент, из-за поворота плитки, worldPos.y может стать кривым.
        Vector3 spawnWorldPos = transform.TransformPoint(randomLocalPos);

        // ТЕПЕРЬ МЫ ЖЕСТКО ФИКСИРУЕМ ВЫСОТУ ПО МИРОВОЙ ОСИ Y.
        // Независимо от того, как повернута плитка, объект встанет точно на Y = 0.
        spawnWorldPos.y = groundedY;

        // 3. Проверка на коллизии (чтобы объекты не спавнились друг в друге).
        // Мы используем маску слоя, чтобы проверка не видела пол плитки.
        if (!Physics.CheckSphere(spawnWorldPos, forbiddenRadius, propsLayer))
        {
            // Выбираем случайный префаб
            GameObject prefab = props[Random.Range(0, props.Length)];
            
            // 4. Спавним объект.
            // Мы НЕ делаем его дочерним (не передаем transform), чтобы он не наследовал
            // кривой поворот плитки. Спавним его в мировом пространстве.
            GameObject instance = Instantiate(prefab, spawnWorldPos, Quaternion.identity);

            // 5. Добавляем случайный поворот ТОЛЬКО по вертикальной оси (Y).
            instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            // 6. Легкий рандом масштаба для разнообразия.
            float scaleMultiplier = Random.Range(0.9f, 1.1f);
            instance.transform.localScale *= scaleMultiplier;
            
            // Опционально: можно переместить объект в какой-нибудь контейнер в иерархии,
            // чтобы не засорять корень сцены.
            // instance.transform.SetParent(GameObject.Find("PropsContainer").transform);
        }
    }

    // Рисуем зоны проверки в редакторе для отладки
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float halfSize = tileSize / 2f;
        Vector3 topLeft = transform.TransformPoint(new Vector3(-halfSize, groundedY, halfSize));
        Vector3 topRight = transform.TransformPoint(new Vector3(halfSize, groundedY, halfSize));
        Vector3 bottomLeft = transform.TransformPoint(new Vector3(-halfSize, groundedY, -halfSize));
        Vector3 bottomRight = transform.TransformPoint(new Vector3(halfSize, groundedY, -halfSize));

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
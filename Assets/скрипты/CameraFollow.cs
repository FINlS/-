using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 15, -10);

    [Header("Настройки движения")]
    public float smoothSpeed = 5f;
    public Vector2 deadzoneSize = new Vector2(3f, 2f);

    [Header("Границы карты (Поле 50x50)")]
    public float minX = -50f;
    public float maxX = 50f;
    public float minZ = -50f;
    public float maxZ = 50f;

    private Vector3 currentTargetPos;

    void Start()
    {
        if (target != null)
        {
            // Сразу ставим камеру в начальную позицию, чтобы не было рывка
            transform.position = target.position + offset;
            currentTargetPos = transform.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Определяем, где камера ДОЛЖНА БЫТЬ относительно игрока
        Vector3 desiredPos = target.position + offset;

        // 2. Логика мертвой зоны: двигаем точку назначения, только если игрок далеко
        float diffX = transform.position.x - desiredPos.x;
        float diffZ = transform.position.z - desiredPos.z;

        if (Mathf.Abs(diffX) > deadzoneSize.x)
        {
            currentTargetPos.x = desiredPos.x + (diffX > 0 ? deadzoneSize.x : -deadzoneSize.x);
        }
        if (Mathf.Abs(diffZ) > deadzoneSize.y)
        {
            currentTargetPos.z = desiredPos.z + (diffZ > 0 ? deadzoneSize.y : -deadzoneSize.y);
        }

        // 3. ПРИНУДИТЕЛЬНОЕ ОГРАНИЧЕНИЕ (Clamp)
        // Теперь камера физически не сможет захотеть точку за пределами min/max
        currentTargetPos.x = Mathf.Clamp(currentTargetPos.x, minX, maxX);
        currentTargetPos.z = Mathf.Clamp(currentTargetPos.z, minZ, maxZ);
        currentTargetPos.y = desiredPos.y; // Высота всегда фиксирована из offset

        // 4. Плавное движение к ограниченной точке
        transform.position = Vector3.Lerp(transform.position, currentTargetPos, Time.deltaTime * smoothSpeed);
    }

    void OnDrawGizmos()
    {
        // Синяя рамка - границы мира
        Gizmos.color = Color.blue;
        Vector3 mapCenter = new Vector3((minX + maxX) / 2f, 0, (minZ + maxZ) / 2f);
        Vector3 mapSize = new Vector3(maxX - minX, 1f, maxZ - minZ);
        Gizmos.DrawWireCube(mapCenter, mapSize);

        if (target != null)
        {
            // Красная рамка - мертвая зона вокруг игрока
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(target.position, new Vector3(deadzoneSize.x * 2, 0.5f, deadzoneSize.y * 2));
        }
    }
}
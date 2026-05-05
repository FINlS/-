using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    public Transform player;

    [Header("Настройки размера")]
    [Tooltip("Чем ВЫШЕ это число, тем КРУПНЕЕ выглядит трава под ногами")]
    public float grassSize = 1000f; 

    private Material groundMat;

    void Start()
    {
        // Кэшируем материал, чтобы не обращаться к нему каждый кадр
        groundMat = GetComponent<Renderer>().material;
    }

    void LateUpdate()
    {
        if (player != null && groundMat != null)
        {
            // 1. Двигаем землю за игроком
            transform.position = new Vector3(player.position.x, 0, player.position.z);

            // 2. Рассчитываем размер (Tiling)
            // Мы делим 1 на grassSize, чтобы при увеличении числа трава росла, а не мельчала
            float tilingValue = 1f / grassSize;
            groundMat.mainTextureScale = new Vector2(tilingValue, tilingValue);

            // 3. Рассчитываем смещение (Offset)
            // Используем ту же переменную, чтобы смещение соответствовало размеру
            Vector2 offset = new Vector2(transform.position.x * tilingValue, transform.position.z * tilingValue);
            groundMat.mainTextureOffset = offset;
        }
    }
}
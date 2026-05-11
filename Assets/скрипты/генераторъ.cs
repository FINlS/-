using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject[] tilePrefabs; // Список твоих плиток
    public Transform player;         // Ссылка на игрока
    public float tileSize = 10f;     // Размер одной плитки
    public int viewDistance = 3;     // На сколько плиток вперед генерировать

    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();

    void Update()
    {
        // Определяем, на какой плитке сейчас стоит игрок
        int playerX = Mathf.FloorToInt(player.position.x / tileSize);
        int playerZ = Mathf.FloorToInt(player.position.z / tileSize);

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int z = -viewDistance; z <= viewDistance; z++)
            {
                Vector2Int tileCoord = new Vector2Int(playerX + x, playerZ + z);

                if (!activeTiles.ContainsKey(tileCoord))
                {
                    SpawnTile(tileCoord);
                }
            }
        }
        // Тут можно добавить удаление очень далеких плиток для оптимизации
    }

    public GameObject altarTilePrefab; // Отдельный префаб плитки с алтарем
    [Range(0, 100)] public float altarChance = 5f; // Шанс 5%

    void SpawnTile(Vector2Int coord)
    {
        Vector3 pos = new Vector3(coord.x * tileSize, 0, coord.y * tileSize);
        GameObject prefab;

        // Решаем, спавнить обычную плитку или алтарь
        if (Random.Range(0f, 100f) < altarChance)
        {
            prefab = altarTilePrefab;
        }
        else
        {
            prefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
        }

        // Случайный поворот на 0, 90, 180 или 270 градусов
        float[] rotations = { 0, 90, 180, 270 };
        float randomYaw = rotations[Random.Range(0, rotations.Length)];
        
        // Сохраняем наклон -90 по X (если твои модели так лежат) и добавляем случайный поворот по Y
        Quaternion rotation = Quaternion.Euler(-90, randomYaw, 0);

        GameObject newTile = Instantiate(prefab, pos, rotation);
        activeTiles.Add(coord, newTile);
    }
}
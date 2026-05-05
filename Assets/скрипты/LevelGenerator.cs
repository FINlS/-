using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Настройки плиток (1x1)")]
    public GameObject tilePrefab; 
    public Material[] grassMaterials; 
    public Material[] stoneMaterials;
    
    [Header("Параметры квадрата")]
    public int size = 100; // Ширина и высота квадрата
    public float tileSize = 0.2f; // Строго 1 для твоих плиток

    void Start()
    {
        GenerateSquare();
    }

    void GenerateSquare()
    {
        // Вычисляем смещение, чтобы центр квадрата был в (0,0,0)
        float offset = (size - 1) / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                // Позиция с учетом смещения для центровки
                Vector3 spawnPos = new Vector3((x - offset) * tileSize, 0, (z - offset) * tileSize);
                
                GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);
                
                ApplyRandomMaterial(newTile);
            }
        }
    }

    void ApplyRandomMaterial(GameObject tile)
    {
        MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
        if (renderer == null) return;

        // 15% камни, остальное трава
        if (Random.value < 0.15f && stoneMaterials.Length > 0)
        {
            renderer.material = stoneMaterials[Random.Range(0, stoneMaterials.Length)];
        }
        else if (grassMaterials.Length > 0)
        {
            renderer.material = grassMaterials[Random.Range(0, grassMaterials.Length)];
        }
    }
}
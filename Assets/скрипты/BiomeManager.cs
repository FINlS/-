using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public enum BiomeType { Start, Desert, Winter, Taiga, Steppe }
    
    public Transform player;
    public float startZoneSize = 10f; // Размер безопасной (зеленой) зоны
    // Внутри BiomeManager.cs
    private BiomeType north, south, east, west;

    void Awake() {
        // Список биомов без стартового
        var types = new System.Collections.Generic.List<BiomeType> { 
            BiomeType.Desert, BiomeType.Winter, BiomeType.Taiga, BiomeType.Steppe 
        };

        // Перемешиваем список (Random Shuffle)
        for (int i = 0; i < types.Count; i++) {
            BiomeType temp = types[i];
            int randomIndex = Random.Range(i, types.Count);
            types[i] = types[randomIndex];
            types[randomIndex] = temp;
        }

        // Назначаем стороны света
        north = types[0];
        south = types[1];
        east = types[2];
        west = types[3];
    }

// Теперь в GetCurrentBiome вместо жестких условий используй эти переменные:
// return pos.x > 0 ? east : west; 
// return pos.z > 0 ? north : south;

    public BiomeType GetCurrentBiome()
    {
        Vector3 pos = player.position;

        // 1. Проверяем, в стартовой ли мы зоне
        if (Mathf.Abs(pos.x) <= startZoneSize && Mathf.Abs(pos.z) <= startZoneSize)
        {
            return BiomeType.Start;
        }

        // 2. Если вышли за пределы, определяем сторону света
        // Сравниваем, какая координата больше отклонилась от нуля
        if (Mathf.Abs(pos.x) > Mathf.Abs(pos.z))
        {
            // Мы сместились больше по горизонтали (Запад или Восток)
            return pos.x > 0 ? east : west;
        }
        else
        {
            // Мы сместились больше по вертикали (Север или Юг)
            return pos.z > 0 ? north : south;
        }
    }
}
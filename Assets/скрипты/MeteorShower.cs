using UnityEngine;

public class MeteorShower : MonoBehaviour
{
    public GameObject redZonePrefab;   // Префаб нашей красной зоны
    public Transform playerTransform;   // Ссылка на игрока
    
    public float spawnRate = 2.0f;      // Как часто появляются зоны
    public float spawnRadius = 6.0f;    // Радиус разброса от игрока
    
    private float nextSpawnTime;

    void Update()
    {
        if (playerTransform == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnDangerZone();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnDangerZone()
    {
        // Случайный сдвиг по оси X (для 2.5D)
        float randomX = Random.Range(-spawnRadius, spawnRadius);
        
        // Красная зона появляется на земле (на уровне ног игрока)
        Vector3 zonePosition = new Vector3(
            playerTransform.position.x + randomX,
            playerTransform.position.y, // На уровне земли
            playerTransform.position.z  // В 2.5D плоскости игрока
        );

        Instantiate(redZonePrefab, zonePosition, Quaternion.identity);
    }
}
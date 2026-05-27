using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [Header("Настройки падающего камня")]
    public GameObject rockPrefab;       // Префаб физического камня
    public float delayBeforeDrop = 1.5f; // Сколько секунд висит красная зона до падения
    public float spawnHeight = 12f;     // Высота, с которой полетит камень
    public float damageRadius = 2.0f;    // Радиус взрыва/урона камня
    public int damageAmount = 20;       // Сколько урона наносит

    [Header("Настройки Звуков")]
    public AudioClip spawnSound;        // Звук предупреждения/свиста (опасность!)
    public AudioClip explosionSound;    // Звук бабаха при приземлении камня
    [Range(0f, 1f)] public float soundVolume = 0.8f; // Общая громкость звуков метеора

    private float timer;
    private bool rockDropped = false;

    void Start()
    {
        timer = delayBeforeDrop;

        // ЗВУК: Как только зона появилась, сразу включаем свист подлетающего метеора
        if (spawnSound != null)
        {
            // Спавним 3D звук прямо в точке этой красной зоны
            AudioSource.PlayClipAtPoint(spawnSound, transform.position, soundVolume);
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && !rockDropped)
        {
            DropRock();
        }
    }

    void DropRock()
    {
        rockDropped = true;

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + spawnHeight, transform.position.z);
        
        if (rockPrefab != null)
        {
            GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);
            
            // Invoke вызовет DealDamage через 0.3 секунды, там же включится и взрыв
            Invoke("DealDamage", 0.3f); 
            
            Destroy(rock, 3f); 
        }

        // Саму зону удаляем чуть позже, чтобы Invoke успел сработать от ее позиции
        Destroy(gameObject, 0.4f);
    }

    void DealDamage()
    {
        // ЗВУК: Камень долетел, наносим урон и включаем БАБАХ!
        if (explosionSound != null)
        {
            // 1. Создаем пустой объект для звука в точке взрыва
            GameObject tempAudioObj = new GameObject("TempExplosionAudio");
            tempAudioObj.transform.position = transform.position;

            // 2. Добавляем на него AudioSource и настраиваем на максимальную громкость
            AudioSource aSource = tempAudioObj.AddComponent<AudioSource>();
            aSource.clip = explosionSound;
        
            // Увеличиваем громкость (можешь поставить 1.5f или 2f, если файл сам по себе тихий!)
            aSource.volume = soundVolume * 1.5f; 

            // ИСПРАВЛЕНИЕ ТИШИНЫ: Сдвигаем ползунок в 2D (0), чтобы расстояние до камеры не глушило взрыв
            aSource.spatialBlend = 0f; 

            // 3. Запускаем воспроизведение
            aSource.Play();

            // 4. Удаляем этот объект со сцены ровно после того, как звук доиграет до конца
            Destroy(tempAudioObj, explosionSound.length);
        }

        // Твоя стандартная логика урона (осталась без изменений)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                hitCollider.GetComponent<Health>().TakeDamage(damageAmount);
                Debug.Log($"Игрок получил {damageAmount} урона от камня!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
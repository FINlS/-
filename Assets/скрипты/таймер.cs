using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Для вывода времени на экран

public class LevelManager : MonoBehaviour
{
    public float timeLimit = 600f; // 10 минут в секундах
    public GameObject portalPrefab; // Префаб портала
    public Transform player;
    public Text timerText; // Опционально: текст таймера на экране

    private bool portalSpawned = false;
    private float currentTime;

    void Start()
    {
        currentTime = timeLimit;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
        }
        else if (!portalSpawned)
        {
            SpawnPortal();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void SpawnPortal()
    {
        portalSpawned = true;
        
        // Спавним портал чуть впереди игрока, чтобы он его сразу заметил
        Vector3 spawnPos = player.position + player.forward * 5f;
        spawnPos.y = 0; // Портал на земле
        
        Instantiate(portalPrefab, spawnPos, Quaternion.identity);
        Debug.Log("Портал открыт! Уходи отсюда!");
    }
}
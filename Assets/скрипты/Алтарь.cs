using UnityEngine;
using TMPro;

public class AltarActivation : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject bossPrefab;    
    public GameObject arenaPrefab;   // Перетащи сюда ПРЕФАБ красных линий из папки Project

    [Header("Точки появления")]
    public Transform bossSpawnPoint; // Точка внутри или рядом с алтарем
    
    [Header("Настройки Взаимодействия")]
    public float interactionDistance = 3f; 
    public GameObject interactionUI; 
    
    private bool isActivated = false;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        
        if (interactionUI != null) interactionUI.SetActive(false);
    }

    void Update()
    {
        if (isActivated || player == null) return;

        if (Vector3.Distance(player.position, transform.position) <= interactionDistance)
        {
            if (interactionUI != null) interactionUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)) ActivateAltar();
        }
        else
        {
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }

    void ActivateAltar()
    {
        isActivated = true;
        if (interactionUI != null) interactionUI.SetActive(false);

        // 1. СПАВНИМ АРЕНУ ВОКРУГ АЛТАРЯ
        if (arenaPrefab != null)
        {
            // Спавним арену ровно в позиции алтаря
            GameObject spawnedArena = Instantiate(arenaPrefab, transform.position, Quaternion.identity);
            
            // Если ты хочешь, чтобы лазеры исчезли после смерти босса, 
            // нам нужно передать ссылку на эту арену боссу.
            // (см. пункт 3 ниже)
        }

        // 2. СПАВНИМ БОССА
        if (bossPrefab != null)
        {
            Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position + Vector3.up * 2f;
            Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        }

        // Эффект света
        Light altarLight = GetComponentInChildren<Light>();
        if (altarLight != null) altarLight.color = Color.red;
    }
}
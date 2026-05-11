using UnityEngine;
using TMPro; // Не забудь добавить этот неймспейс, если используешь TextMeshPro

public class AltarActivation : MonoBehaviour
{
    [Header("Настройки Босса")]
    public GameObject bossPrefab;    
    public Transform spawnPoint;     
    
    [Header("Настройки Взаимодействия")]
    public float interactionDistance = 3f; 
    public GameObject interactionUI; // Перетащи сюда объект с текстом "Нажми E"
    
    private bool isActivated = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Скрываем надпись при старте
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void Update()
    {
        if (isActivated) 
        {
            // Если алтарь уже нажат, надпись больше не нужна
            if (interactionUI != null && interactionUI.activeSelf)
                interactionUI.SetActive(false);
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance)
        {
            // Показываем надпись, если игрок рядом
            if (interactionUI != null) interactionUI.SetActive(true);

            // Проверяем нажатие
            if (Input.GetKeyDown(KeyCode.E))
            {
                ActivateAltar();
            }
        }
        else
        {
            // Прячем надпись, если игрок отошел
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }

    void ActivateAltar()
    {
        isActivated = true;
        
        if (interactionUI != null) interactionUI.SetActive(false);

        if (bossPrefab != null)
        {
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        }

        // Эффект активации (красный свет)
        Light altarLight = GetComponentInChildren<Light>();
        if (altarLight != null) altarLight.color = Color.red;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
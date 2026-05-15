using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Используем TextMeshPro

public class Portal25D : MonoBehaviour
{
    [Header("Настройки сцены")]
    public string sceneToLoad;

    [Header("UI элементы")]
    public GameObject uiPanel;      // Объект с текстом
    public TextMeshProUGUI tmpText; // Ссылка на сам компонент текста

    [Header("Тексты")]
    public string textNoKey = "Нужен ключ, чтобы войти...";
    public string textHasKey = "Нажми [E], чтобы войти в портал";

    private bool playerInRange = false;
    private Inventory playerInventory;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (playerInventory != null && playerInventory.hasKey)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    // Когда игрок входит в зону
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInventory = other.GetComponent<Inventory>();
            playerInRange = true;
            uiPanel.SetActive(true);

            // Проверка текста
            if (playerInventory != null && playerInventory.hasKey)
                tmpText.text = textHasKey;
            else
                tmpText.text = textNoKey;
        }
    }

    // Когда игрок выходит из зоны
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            uiPanel.SetActive(false);
        }
    }
}
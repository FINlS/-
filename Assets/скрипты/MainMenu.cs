using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Панели интерфейса")]
    public GameObject mainPanel;      // Объект с главными кнопками
    public GameObject skinsPanel;     // Объект "Скины" из твоей иерархии
    public GameObject inventoryPanel; // Объект "инвент" из твоей иерархии

    void Start()
    {
        // При запуске всегда показываем только главное меню
        ShowMain();
    }

    // Метод для кнопки "Играть"
    public void PlayGame()
    {
        // Загружаем индекс 1 (убедись, что игровая сцена добавлена в Build Settings)
        SceneManager.LoadScene(1); 
    }

    // Переход в Скины
    public void ShowSkins()
    {
        mainPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        skinsPanel.SetActive(true);
    }

    // Переход в Инвентарь
    public void ShowInventory()
    {
        mainPanel.SetActive(false);
        skinsPanel.SetActive(false);
        inventoryPanel.SetActive(true);
    }

    // Возврат в главное меню
    public void ShowMain()
    {
        skinsPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Игра закрыта");
    }
}
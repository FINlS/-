using UnityEngine;
using System.Collections; // ОБЯЗАТЕЛЬНО для работы корутин и ожидания
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject gameOverPanel; // Твоя GameOverPanel

    [Header("Настройки сцены")]
    public string inventorySceneName = "Inventory"; // Имя сцены для перехода

    void Start()
    {
        Time.timeScale = 1f; 
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Этот метод теперь просто запускает безопасное ожидание
    public void ShowGameOverScreen()
    {
        StartCoroutine(WaitAndOpenScreen());
    }

    // Корутина, которая делает паузу перед включением UI
    IEnumerator WaitAndOpenScreen()
    {
        Debug.Log("[Game Over] Игрок умирает, ждем 3 секунды анимации...");
        
        // Ждем 3 секунды (игра в это время ЕЩЕ НЕ НА ПАУЗЕ, всё красиво движется)
        yield return new WaitForSeconds(3f);

        // Этот код сработает строго ПОСЛЕ 3 секунд ожидания:
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Включаем панель «Ты умер»

            // Только СЕЙЧАС ставим игру на паузу
            Time.timeScale = 0f; 

            // Включаем мышку для игрока
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("[Game Over] Панель активирована, игра поставлена на паузу.");
        }
    }

    // Метод для кнопки Выход
    public void ExitToInventoryScene()
    {
        Debug.Log($"[Game Over] Переходим на сцену: {inventorySceneName}");
        
        // Отжимаем паузу времени, иначе следующая сцена зависнет!
        Time.timeScale = 1f; 

        // Загружаем сцену инвентаря
        SceneManager.LoadScene(inventorySceneName);
    }
}
using UnityEngine;
using UnityEngine.UI; // ОБЯЗАТЕЛЬНО для работы с UI компонентами
using UnityEngine.SceneManagement; // ОБЯЗАТЕЛЬНО для переключения сцен

public class VictoryManager : MonoBehaviour
{
    [Header("Ссылки на UI элементы")]
    public GameObject victoryPanel;   // Перетащи сюда твою панель победы (VictoryPanel)
    public Button nextLevelButton;    // (Опционально) Ссылка на кнопку следующего уровня

    [Header("Настройки сцен")]
    public string nextSceneName = "Level3";      // Имя сцены следующего уровня
    public string mainMenuSceneName = "Inventory"; // Имя сцены главного меню или инвентаря

    void Start()
    {
        // При старте уровня экран победы гарантированно должен быть выключен
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("[VictoryManager] Забыл перетащить VictoryPanel в Инспектор!");
        }
    }

    // Этот метод вызывается из скрипта босса (BossHealth) после его смерти
    public void TriggerVictory()
    {
        if (victoryPanel != null)
        {
            // 1. Включаем экран победы
            victoryPanel.SetActive(true);

            // 2. Замораживаем время в игре (чтобы враги/снаряды застыли)
            Time.timeScale = 0f;

            // 3. Включаем и разблокируем курсор мыши
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("[Victory] Финал игры! Экран победы успешно активирован.");
        }
    }
}
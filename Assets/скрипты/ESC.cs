using UnityEngine;
using UnityEngine.UI; // ОБЯЗАТЕЛЬНО для работы со слайдерами и UI
using UnityEngine.SceneManagement; // ОБЯЗАТЕЛЬНО для переключения сцен

public class PauseMenuManager : MonoBehaviour
{
    [Header("Ссылки на UI элементы")]
    public GameObject pauseMenuPanel; // Ссылка на саму панель меню паузы
    public Slider sensitivitySlider;   // Ссылка на ползунок чувствительности

    [Header("Настройки сцены")]
    public string inventorySceneName = "Inventory"; // Имя сцены инвентаря/меню
    public string NextSceneName = "Next"; // Имя сцены инвентаря/меню

    private bool isPaused = false;

    void Start()
    {
        // 1. При старте игры панель паузы обязательно должна быть скрыта
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // 2. Настраиваем ползунок чувствительности
        if (sensitivitySlider != null)
        {
            // Загружаем сохраненное значение из памяти. Если его нет, по умолчанию ставим 200
            float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 200f);
            sensitivitySlider.value = savedSensitivity;

            // Заставляем ползунок вызывать метод изменения при каждом его сдвиге
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }
        else
        {
            Debug.LogWarning("[PauseMenu] Ползунок SensitivitySlider не привязан в инспекторе!");
        }
    }

    void Update()
    {
        // Отслеживаем нажатие клавиши ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // Если уже на паузе — возвращаемся в игру
            }
            else
            {
                PauseGame(); // Если играли — ставим на паузу
            }
        }
    }

    // --- ЛОГИКА ОКНА ПАУЗЫ ---

    public void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true); // Показываем меню
            Time.timeScale = 0f;            // Полностью замораживаем время в игре
            isPaused = true;

            // Включаем и освобождаем курсор мыши, чтобы можно было кликать
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Debug.Log("[Пауза] Игра приостановлена.");
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false); // Скрываем меню
            Time.timeScale = 1f;             // Возвращаем время в обычный режим
            isPaused = false;

            // Прячем курсор обратно в игру (для шутера от 1 лица)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("[Пауза] Игра возобновлена.");
        }
    }

    // --- ЛОГИКА НАСТРОЙКИ ЧУВСТВИТЕЛЬНОСТИ ---

    public void SetSensitivity(float value)
    {
        // Сохраняем новое значение ползунка в память устройства
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save(); 

        // Ищем активного игрока на сцене и мгновенно меняем ему чувствительность
        MouseLook playerMouse = FindFirstObjectByType<MouseLook>();
        if (playerMouse != null)
        {
            playerMouse.mouseSensitivity = value;
        }
        
        Debug.Log($"[Пауза] Чувствительность мыши изменена на: {value}");
    }

    // --- ЛОГИКА КНОПОК ---

    // Метод для кнопки «В инвентарь»
    public void GoToInventory()
    {
        Time.timeScale = 1f; // КРИТИЧЕСКИ ВАЖНО: возвращаем время в норму перед сменой сцены!
        SceneManager.LoadScene(inventorySceneName);
    }
    public void GoNext()
    {
        Time.timeScale = 1f; // КРИТИЧЕСКИ ВАЖНО: возвращаем время в норму перед сменой сцены!
        SceneManager.LoadScene(NextSceneName);
    }
    // Метод для кнопки «Выйти из игры»
    public void QuitGame()
    {
        Debug.Log("[Пауза] Закрытие приложения...");
        Application.Quit();

        #if UNITY_EDITOR
        // Строка ниже заставит кнопку работать прямо внутри редактора Unity
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
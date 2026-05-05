using UnityEngine;
using UnityEngine.SceneManagement; // Нужно для перезагрузки сцены

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI; // Перетащи сюда панель Game Over из UI

    public void EndGame()
    {
        Debug.Log("Игра окончена!");
        
        // 1. Показываем меню проигрыша
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. Останавливаем время в игре
        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        // Снова запускаем время и перезагружаем текущую сцену
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Ссылки на UI элементы")]
    public GameObject tutorialPanel; // Ссылка на панель с подсказками

    [Header("Настройки времени")]
    public float displayDuration = 10f; // Сколько секунд показывать подсказку

    void Start()
    {
        // Проверяем, назначена ли панель в инспекторе
        if (tutorialPanel != null)
        {
            // На всякий случай принудительно включаем её в самом начале
            tutorialPanel.SetActive(true);

            // Запускаем корутину (таймер) для автоматического скрытия
            StartCoroutine(HideTutorialAfterDelay());
        }
        else
        {
            Debug.LogError("[TutorialManager] Не назначена Tutorial Panel в инспекторе!");
        }
    }

    IEnumerator HideTutorialAfterDelay()
    {
        // Ждем ровно указанное количество секунд
        yield return new WaitForSeconds(displayDuration);

        // Выключаем панель с экрана
        tutorialPanel.SetActive(false);
        
        Debug.Log("[Tutorial] Подсказки успешно скрыты.");
    }
}
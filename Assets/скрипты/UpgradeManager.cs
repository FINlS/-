using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Нужно для работы со списками

public class UpgradeManager : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public GameObject levelUpPanel;
    public Button[] upgradeButtons; 
    public TextMeshProUGUI[] buttonTexts; 

    [Header("Ссылка на игрока")]
    public WeaponController weapon;

    void Start()
    {
        // Скрываем меню при старте игры
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    // Этот метод вызывается из PlayerStats при достижении нового уровня
    public void ShowUpgradeMenu()
    {
        if (weapon == null) weapon = FindObjectOfType<WeaponController>();

        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // ПАУЗА ИГРЫ

        // Создаем список доступных ID улучшений (0, 1, 2)
        List<int> availableUpgrades = new List<int> { 0, 1, 2 };

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            // Выбираем случайный тип из списка доступных
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            int selectedType = availableUpgrades[randomIndex];

            // Удаляем выбранный тип из временного списка, чтобы он не повторился на другой кнопке
            availableUpgrades.RemoveAt(randomIndex);

            // Настраиваем кнопку
            SetupButton(i, selectedType);
        }
    }

    void SetupButton(int buttonIndex, int upgradeType)
    {
        // Очищаем старые функции с кнопки
        upgradeButtons[buttonIndex].onClick.RemoveAllListeners();

        // Назначаем текст и логику в зависимости от типа
        switch (upgradeType)
        {
            case 0:
                buttonTexts[buttonIndex].text = "СКОРОСТРЕЛЬНОСТЬ\n+25%";
                upgradeButtons[buttonIndex].onClick.AddListener(() => {
                    weapon.fireRate *= 0.75f; // Уменьшаем задержку между выстрелами
                    ResumeGame();
                });
                break;

            case 1:
                buttonTexts[buttonIndex].text = "ТОЧНОСТЬ\n+40%";
                upgradeButtons[buttonIndex].onClick.AddListener(() => {
                    weapon.spread *= 0.6f; // Уменьшаем разброс пуль
                    ResumeGame();
                });
                break;

            case 2:
                buttonTexts[buttonIndex].text = "ДАЛЬНОСТЬ АВТОБОЯ\n+5 МЕТРОВ";
                upgradeButtons[buttonIndex].onClick.AddListener(() => {
                    weapon.autoRange += 5f; // Увеличиваем радиус поиска врагов
                    ResumeGame();
                });
                break;
        }
    }

    public void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // СНИМАЕМ ПАУЗУ
    }
}
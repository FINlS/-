using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public float currentExp = 0;
    public float expToLevelUp = 100;

    public Slider expBar; // Перетащи сюда слайдер из UI
    // Добавь это в начало скрипта PlayerStats
    public UpgradeManager upgradeManager;

    // Обнови метод LevelUp
    void LevelUp()
    {
        level++;
        currentExp -= expToLevelUp;
        expToLevelUp *= 1.25f; // Увеличиваем порог следующего уровня

        UpdateUI();

        // Вызываем меню выбора
        if (upgradeManager != null)
        {
            upgradeManager.ShowUpgradeMenu();
        }
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;

        if (currentExp >= expToLevelUp)
        {
            LevelUp();
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (expBar != null)
        {
            expBar.value = currentExp / expToLevelUp;
        }
    }
}
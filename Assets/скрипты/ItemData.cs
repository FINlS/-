using UnityEngine;

// Позволяет создавать новые предметы через меню: Right Click -> Inventory -> Item
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;      // Название предмета
    [TextArea] public string description; // Описание
    public Sprite icon;          // Иконка для сетки (Grid)
    public GameObject itemPrefab; // Сюда в инспекторе перетащи 3D-модель автомата
    // Типы предметов для слотов в стиле "Ведьмака"
    public enum ItemType { Helmet, Armor, Weapon, Other }
    public ItemType type;

    public int power;            // Характеристика (урон или защита)
}
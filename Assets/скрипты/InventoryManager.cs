using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Слоты экипировки")]
    public InventorySlot Helmet;
    public InventorySlot Armor;
    public InventorySlot Weapon;
    [Header("Точки крепления на персонаже")]
    public Transform weaponHand; // Пустой объект в руке 3D-модели персонажа
    void Awake()
    {
        Instance = this;
    }

    public void EquipItem(ItemData itemToEquip)
    {
        // Проверяем тип предмета и отправляем в нужный слот
        switch (itemToEquip.type)
        {
            case ItemData.ItemType.Helmet:
                Helmet.SetItem(itemToEquip);
                break;
            case ItemData.ItemType.Armor:
                Armor.SetItem(itemToEquip);
                break;
            case ItemData.ItemType.Weapon:
                Weapon.SetItem(itemToEquip);
                break;
        }
        Debug.Log("Надето: " + itemToEquip.itemName);
        if (itemToEquip.type == ItemData.ItemType.Weapon)
    {
        // Удаляем старое оружие из рук, если оно есть
        foreach (Transform child in weaponHand) {
            Destroy(child.gameObject);
        }
        
        // Создаем новое оружие в руке
        if (itemToEquip.itemPrefab != null) {
            Instantiate(itemToEquip.itemPrefab, weaponHand);
        }
    }
    }
}
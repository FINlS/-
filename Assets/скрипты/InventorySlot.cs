using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemData item;
    public Image iconChild;
    public bool isEquipmentSlot;
    public void SetItem(ItemData newItem)
    {
        item = newItem;
        if (item != null)
        {
            iconChild.sprite = item.icon;
            iconChild.enabled = true;
        }
        else
        {
            iconChild.enabled = false;
        }
    }

    public void OnSlotClick()
    {
        if (item != null)
        {
            if (isEquipmentSlot) // Добавь такую переменную-галочку в скрипт
            {
                // Логика снятия: очищаем слот
                SetItem(null); 
                Debug.Log("Предмет снят");
            }
            else
            {
                // Логика надевания
                InventoryManager.Instance.EquipItem(item);
            }
        }
    }
}
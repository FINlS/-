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
        if (item == null) return;

        // Вместо поиска EquipmentManager напрямую, вызываем InventoryManager
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.EquipItem(item);
            Debug.Log($"[InventorySlot] Клик по предмету: {item.itemName}");
        }
        else
        {
            Debug.LogError("InventoryManager.Instance не найден!");
        }
    }
}
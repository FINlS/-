using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI Слоты (могут быть пустые в самой игре)")]
    public InventorySlot weaponSlot1UI;
    public InventorySlot weaponSlot2UI;
    public InventorySlot helmetSlotUI;

    [Header("База данных всех предметов")]
    public ItemData[] allItems; 

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // При запуске любой сцены (меню или игра) загружаем то, что было выбрано
        LoadEquipment();
    }

    public void EquipItem(ItemData item)
    {
        if (item == null) return;

        if (item.type == ItemData.ItemType.Weapon)
        {
            HandleWeapon(item);
        }
        else if (item.type == ItemData.ItemType.Helmet)
        {
            HandleHelmet(item);
        }
        
        // После любого клика в меню — сохраняем
        SaveEquipment(); 
    }

    private void HandleWeapon(ItemData item)
    {
        if (string.IsNullOrEmpty(item.itemName)) item.itemName = item.name;

        Transform currentSocket = FindSocketByItemName(item.itemName);

        if (currentSocket != null)
        {
            EquipmentManager.Instance.ClearSocket(currentSocket);
            UpdateUI(item, true);
        }
        else
        {
            Transform targetSocket = GetFreeWeaponSocket();
            EquipmentManager.Instance.SpawnItem(item.itemPrefab, targetSocket, item.itemName);
            UpdateUI(item, false);
        }
    }

    private void HandleHelmet(ItemData item)
    {
        if (string.IsNullOrEmpty(item.itemName)) item.itemName = item.name;

        if (EquipmentManager.Instance.headSocket.Find(item.itemName) != null)
        {
            EquipmentManager.Instance.ClearSocket(EquipmentManager.Instance.headSocket);
            UpdateUI(item, true);
        }
        else
        {
            EquipmentManager.Instance.SpawnItem(item.itemPrefab, EquipmentManager.Instance.headSocket, item.itemName);
            UpdateUI(item, false);
        }
    }

    private void UpdateUI(ItemData item, bool isRemoving)
    {
        // Проверка на null, так как в сцене игры UI слотов может не быть
        if (item.type == ItemData.ItemType.Weapon)
        {
            if (weaponSlot1UI != null && (weaponSlot1UI.item == item || weaponSlot1UI.item == null)) 
                weaponSlot1UI.SetItem(isRemoving ? null : item);
            else if (weaponSlot2UI != null) 
                weaponSlot2UI.SetItem(isRemoving ? null : item);
        }
        else if (item.type == ItemData.ItemType.Helmet && helmetSlotUI != null)
        {
            helmetSlotUI.SetItem(isRemoving ? null : item);
        }
    }

    // --- СИСТЕМА ПЕРЕНОСА МЕЖДУ СЦЕНАМИ ---

    public void SaveEquipment()
    {
        PlayerPrefs.SetString("Save_RightHand", GetNameFromSocket(EquipmentManager.Instance.rightHandSocket));
        PlayerPrefs.SetString("Save_LeftHand", GetNameFromSocket(EquipmentManager.Instance.leftHandSocket));
        PlayerPrefs.SetString("Save_Helmet", GetNameFromSocket(EquipmentManager.Instance.headSocket));
        PlayerPrefs.Save();
    }

    private string GetNameFromSocket(Transform socket)
    {
        if (socket != null && socket.childCount > 0) return socket.GetChild(0).name;
        return "";
    }

    public void LoadEquipment()
    {
        string right = PlayerPrefs.GetString("Save_RightHand", "");
        string left = PlayerPrefs.GetString("Save_LeftHand", "");
        string helmet = PlayerPrefs.GetString("Save_Helmet", "");

        foreach (ItemData data in allItems)
        {
            // Если имя предмета совпадает с любым из сохраненных — надеваем его физически
            if (data.itemName == right || data.itemName == left || data.itemName == helmet)
            {
                // Вызываем напрямую физический спавн, чтобы не зациклить сохранение
                if (data.type == ItemData.ItemType.Weapon)
                {
                    Transform socket = (data.itemName == right) ? EquipmentManager.Instance.rightHandSocket : EquipmentManager.Instance.leftHandSocket;
                    EquipmentManager.Instance.SpawnItem(data.itemPrefab, socket, data.itemName);
                }
                else if (data.type == ItemData.ItemType.Helmet)
                {
                    EquipmentManager.Instance.SpawnItem(data.itemPrefab, EquipmentManager.Instance.headSocket, data.itemName);
                }
                
                // Обновляем UI, если мы в сцене меню
                UpdateUI(data, false);
            }
        }
    }

    private Transform FindSocketByItemName(string itemName)
    {
        if (EquipmentManager.Instance.rightHandSocket.Find(itemName)) return EquipmentManager.Instance.rightHandSocket;
        if (EquipmentManager.Instance.leftHandSocket.Find(itemName)) return EquipmentManager.Instance.leftHandSocket;
        return null;
    }

    private Transform GetFreeWeaponSocket()
    {
        if (EquipmentManager.Instance.rightHandSocket.childCount == 0) return EquipmentManager.Instance.rightHandSocket;
        if (EquipmentManager.Instance.leftHandSocket.childCount == 0) return EquipmentManager.Instance.leftHandSocket;
        return EquipmentManager.Instance.rightHandSocket;
    }
}
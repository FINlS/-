using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("Точки крепления (Sockets)")]
    public Transform rightHandSocket;
    public Transform leftHandSocket;
    public Transform headSocket;

    void Awake()
    {
        Instance = this; // Перезаписываем ссылку на актуальный менеджер в новой сцене
    }

    // Спавн предмета в конкретный сокет
    public void SpawnItem(GameObject prefab, Transform socket, string itemName)
    {
        ClearSocket(socket);

        if (prefab != null)
        {
            GameObject newItem = Instantiate(prefab, socket);
            newItem.name = itemName; // Важно для поиска и сохранения
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;
            newItem.transform.localScale = prefab.transform.localScale;        }
    }

    // Удаление всех предметов из сокета
    public void ClearSocket(Transform socket)
    {
        if (socket == null) return;
        foreach (Transform child in socket)
        {
            Destroy(child.gameObject);
        }
    }
}
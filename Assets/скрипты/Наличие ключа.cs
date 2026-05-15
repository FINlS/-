using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inv = other.GetComponent<Inventory>();
            if (inv != null)
            {
                inv.hasKey = true; // Даем ключ
                Debug.Log("Ключ получен!");
                Destroy(gameObject); // Удаляем ключ со сцены
            }
        }
    }
}
using UnityEngine;
using TMPro; // Если используешь обычный текст, замени на using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 4f; // Расстояние взаимодействия
    public GameObject interactUI;          // Ссылка на объект текста "Нажми E"

    void Update()
    {
        RaycastHit hit;
        // Пускаем луч из центра камеры вперед
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            // Проверяем тег объекта, в который попал луч
            if (hit.collider.CompareTag("Portal"))
            {
                interactUI.SetActive(true); // Показываем надпись

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Ищем скрипт Portal на этом объекте или его родителях
                    Portal portal = hit.collider.GetComponentInParent<Portal>();
                    if (portal != null)
                    {
                        portal.ActivatePortal();
                    }
                }
            }
            else
            {
                interactUI.SetActive(false); // Скрываем, если смотрим не на портал
            }
        }
        else
        {
            interactUI.SetActive(false); // Скрываем, если смотрим в пустоту
        }
    }
}
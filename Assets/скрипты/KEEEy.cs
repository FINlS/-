using UnityEngine;

public class KeyItem2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что ключ задел именно игрок
        if (other.CompareTag("Player"))
        {
            // Ищем объект стрелочки внутри игрока. 
            // transform.Find ищет дочерний объект по его имени в иерархии персонажа.
            Transform arrowTransform = other.transform.Find("PortalArrow");

            if (arrowTransform != null)
            {
                // Включаем стрелочку!
                arrowTransform.gameObject.SetActive(true);
                Debug.Log("[Ключ] Игрок подобрал ключ, стрелка к порталу включена!");
            }
            else
            {
                Debug.LogError("[Ключ] Не удалось найти дочерний объект с именем 'PortalArrow' внутри Игрока!");
            }

            // Удаляем ключ со сцены (ведь мы его подобрали)
            Destroy(gameObject);
        }
    }
}
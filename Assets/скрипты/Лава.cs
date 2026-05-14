using UnityEngine;

public class LavaDeath : MonoBehaviour
{
    // Сюда в Инспекторе перетащи объект игрока или точку старта
    public Transform spawnPoint; 

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что в лаву упал именно игрок
        if (other.CompareTag("Player"))
        {
            // Если используешь CharacterController, его нужно на миг выключить, 
            // иначе телепортация может не сработать из-за внутренней физики
            CharacterController cc = other.GetComponent<CharacterController>();
            
            if (cc != null) cc.enabled = false;

            // Перемещаем игрока в точку спавна
            other.transform.position = spawnPoint.position;

            if (cc != null) cc.enabled = true;
            
            Debug.Log("Игрок сгорел в лаве и возродился!");
        }
    }
}
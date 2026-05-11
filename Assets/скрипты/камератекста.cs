using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        // Вариант А: Текст всегда смотрит на камеру (эффект билборда)
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
        
        // Вариант Б: Текст просто всегда имеет нулевой мировой поворот
        // transform.rotation = Quaternion.Euler(0, 0, 0); 
    }
}
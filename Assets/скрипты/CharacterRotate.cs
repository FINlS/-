using UnityEngine;

public class CharacterRotate : MonoBehaviour
{
    public float sensitivity = 5f; // Скорость вращения
    private bool isRotating = false;

    void Update()
    {
        // Начинаем вращение при нажатии левой кнопки мыши (LMB)
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
        }

        // Останавливаем, когда отпускаем кнопку
        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            // Получаем движение мыши по горизонтали
            float mouseX = Input.GetAxis("Mouse X");

            // Вращаем персонажа вокруг вертикальной оси (Y)
            // Инвертируем значение (-mouseX), чтобы модель крутилась вслед за мышкой
            transform.Rotate(Vector3.up, -mouseX * sensitivity * 100 * Time.deltaTime);
        }
    }
}
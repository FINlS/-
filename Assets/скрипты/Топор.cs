using UnityEngine;

public partial class Pendulum : MonoBehaviour
{
    [Header("Настройки качания")]
    public float angle = 45f;      // Максимальный угол отклонения в одну сторону
    public float speed = 2f;      // Скорость качания
    public Vector3 axis = Vector3.forward; // Ось, вокруг которой качается топор

    private Quaternion startRotation;

    void Start()
    {
        // Запоминаем начальное положение топора
        startRotation = transform.localRotation;
    }

    void Update()
    {
        // Вычисляем угол в текущий момент времени
        // Mathf.Sin возвращает значение от -1 до 1
        float currentAngle = Mathf.Sin(Time.time * speed) * angle;

        // Применяем вращение к начальному повороту
        transform.localRotation = startRotation * Quaternion.AngleAxis(currentAngle, axis);
    }
}
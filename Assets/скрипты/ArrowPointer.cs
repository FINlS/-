using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    [Header("Цель")]
    public Transform portalTransform; // Ссылка на портал, куда указывать

    [Header("Настройки парения стрелочки")]
    public float rotationSpeed = 10f; // Скорость разворота стрелки
    public float floatSpeed = 3f;     // Скорость покачивания вверх-вниз
    public float floatAmplitude = 0.2f;// Высота покачивания

    private Vector3 startLocalPosition;

    void Start()
    {
        // Запоминаем начальную позицию над головой
        startLocalPosition = transform.localPosition;

        // Если забыл указать портал в инспекторе, попробуем найти его по тегу
        if (portalTransform == null)
        {
            GameObject portal = GameObject.FindGameObjectWithTag("Portal");
            if (portal != null) portalTransform = portal.transform;
        }

        // В начале игры стрелочка должна быть выключена.
        // Она включится из скрипта ключа, когда игрок его подберет!
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (portalTransform == null) return;

        // 1. ПОВОРОТ В СТОРОНУ ПОРТАЛА
        // Считаем направление до портала, игнорируя высоту (ось Y)
        Vector3 targetDirection = portalTransform.position - transform.position;
        targetDirection.y = 0; 

        if (targetDirection != Vector3.zero)
        {
            // Вычисляем нужный поворот
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            // Плавно разворачиваем стрелку
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 2. КРАСИВОЕ ПАРЕНИЕ (Эффект Hover)
        // Считаем смещение по синусоиде от времени
        float newY = startLocalPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = new Vector3(startLocalPosition.x, newY, startLocalPosition.z);
    }
}
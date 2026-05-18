using UnityEngine;

public partial class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        // Скрываем курсор и блокируем его в центре экрана
        Cursor.lockState = CursorLockMode.Locked;

        // ЗАГРУЗКА НАСТРОЙКИ: Проверяем, сохранял ли игрок чувствительность ранее
        // Если да — загружаем, если нет — оставляем стандартные 200
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
    }

    void Update()
    {
        // Если игра на паузе (активно меню ESC), мышь не должна крутить камеру
        if (Time.timeScale == 0f) return;

        // Получаем ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вычисляем вращение по вертикали (вверх-вниз)
        xRotation -= mouseY;
        // Ограничиваем вращение, чтобы нельзя было "сделать сальто" головой
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Применяем вращение к камере
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Поворачиваем всё тело игрока (капсулу) влево-вправо
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
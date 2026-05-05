using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{
    public enum FireMode { Manual, Auto }

    [Header("Режим и Интерфейс")]
    public FireMode currentMode = FireMode.Manual;
    public TextMeshProUGUI uiModeText;

    [Header("Ссылки на объекты")]
    public GameObject bulletPrefab;
    public Transform firePoint;      // Точка вылета на дуле
    public Transform playerTransform; // Объект игрока (родитель)

    [Header("Баланс стрельбы")]
    public float fireRate = 0.2f;    // Пауза между выстрелами
    public float autoRange = 10f;    // Радиус поиска врагов
    public float rotationSpeed = 15f; // Насколько быстро крутится персонаж

    [Header("Механика Разброса")]
    [Range(0, 45)] 
    public float spread = 45f;        // Угол разброса в градусах

    private float nextFireTime;
    private Transform autoTarget;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        // Если игрок не назначен, ищем корневой объект
        if (playerTransform == null) playerTransform = transform.root;
    }

    void Update()
    {
        // Переключение режима на кнопку Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentMode = (currentMode == FireMode.Manual) ? FireMode.Auto : FireMode.Manual;
        }

        // Обновляем текст режима
        if (uiModeText != null) 
            uiModeText.text = "MODE: " + currentMode.ToString();

        // Логика прицеливания
        if (currentMode == FireMode.Manual)
        {
            HandleManualInput();
        }
        else
        {
            HandleAutoInput();
        }
    }

    private void HandleManualInput()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // Создаем плоскость на высоте пушки, чтобы расчет был точным
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            AimAt(targetPoint);

            // Ручная стрельба (зажатая ЛКМ)
            if (Input.GetMouseButton(0))
            {
                TryShoot();
            }
        }
    }

    private void HandleAutoInput()
    {
        FindClosestEnemy();

        if (autoTarget != null)
        {
            AimAt(autoTarget.position);
            TryShoot(); // Авто-стрельба
        }
    }

    private void AimAt(Vector3 targetPoint)
    {
        // 1. Поворот ИГРОКА (корпуса) только влево-вправо
        Vector3 playerDir = targetPoint - playerTransform.position;
        playerDir.y = 0;
        if (playerDir.magnitude > 0.1f)
        {
            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, Quaternion.LookRotation(playerDir), Time.deltaTime * rotationSpeed);
        }

        // 2. Поворот ПУШКИ (сведение к цели) только влево-вправо
        Vector3 gunDir = targetPoint - transform.position;
        gunDir.y = 0; // Строго горизонтально (убираем наклон по Y)
        
        if (gunDir.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(gunDir), Time.deltaTime * rotationSpeed);
        }

        // Рисуем линию прицеливания в окне Scene
        Debug.DrawLine(firePoint.position, targetPoint, Color.green);
    }

    private void TryShoot()
    {
        if (Time.time >= nextFireTime && bulletPrefab != null)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        // Рассчитываем случайный разброс
        float randomSpread = Random.Range(-spread, spread);
        
        // Берем текущий поворот пушки и добавляем смещение по оси Y
        Quaternion spreadRotation = transform.rotation * Quaternion.Euler(0, randomSpread, 0);

        // Создаем пулю
        Instantiate(bulletPrefab, firePoint.position, spreadRotation);
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDist = Mathf.Infinity;
        autoTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist && dist <= autoRange)
            {
                closestDist = dist;
                autoTarget = enemy.transform;
            }
        }
    }
}
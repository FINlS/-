using UnityEngine;
using TMPro;

public class WeaponControllerFP : MonoBehaviour
{
    public enum FireMode { Semi, Auto }

    [Header("Режим и Интерфейс")]
    public FireMode currentMode = FireMode.Semi;
    public TextMeshProUGUI uiModeText;

    [Header("Ссылки на объекты")]
    public GameObject bulletPrefab;
    public Transform firePoint;      
    private Camera cam;              

    [Header("Баланс стрельбы")]
    public float fireRate = 0.1f;    
    public float damage = 10f;       

    [Header("Механика Разброса (FPS)")]
    [Range(0, 5)] 
    public float spread = 0.5f;      

    private float nextFireTime;

    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Защита: если камера потерялась, пробуем найти её снова
        if (cam == null) cam = Camera.main;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentMode = (currentMode == FireMode.Semi) ? FireMode.Auto : FireMode.Semi;
        }

        if (uiModeText != null) 
            uiModeText.text = "FIRE MODE: " + currentMode.ToString();

        // Отрисовка лазера должна быть здесь, чтобы он обновлялся КАЖДЫЙ кадр
        DrawLaserSight();

        HandleInput();
    }

    private void DrawLaserSight()
    {
        if (cam == null || firePoint == null) return;

        // ВАЖНО: Ray создается ВНУТРИ метода, чтобы он обновлялся при каждом повороте головы
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(100);

        Debug.DrawLine(firePoint.position, targetPoint, Color.red);
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || cam == null) return;

        // Снова создаем луч, чтобы получить свежую точку в момент выстрела
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(100);

        Vector3 directionToTarget = (targetPoint - firePoint.position).normalized;

        // Модифицируем направление разбросом
        Vector3 finalDirection = directionToTarget;
        finalDirection.x += Random.Range(-spread / 100f, spread / 100f);
        finalDirection.y += Random.Range(-spread / 100f, spread / 100f);

        Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(finalDirection));
    }

        private void HandleInput()
    {
        bool isFiring = (currentMode == FireMode.Auto) ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (isFiring && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
}
using UnityEngine;

public class BiomeColorController : MonoBehaviour
{
    public Transform player;
    public MeshRenderer groundRenderer; // Сюда перетащи плоскость земли
    public float transitionSpeed = 2f;  // Скорость смены цвета
    public float startZoneSize = 10f;   // Размер начального квадрата

    [Header("Цвета Биомов")]
    public Color startColor = Color.green;
    public Color desertColor = new Color(0.9f, 0.8f, 0.4f); // Песочный
    public Color winterColor = Color.white;
    public Color taigaColor = new Color(0.1f, 0.4f, 0.1f);  // Темно-зеленый
    public Color steppeColor = new Color(0.6f, 0.7f, 0.3f); // Сухая трава

    private Color targetColor;
    private Material groundMat;

    // Храним, какой биом в какой стороне (для рандома)
    private Color northColor, southColor, eastColor, westColor;

    void Start()
    {
        groundMat = groundRenderer.material;
        AssignRandomBiomes();
    }

    void AssignRandomBiomes()
    {
        // Список доступных цветов
        var colors = new System.Collections.Generic.List<Color> { desertColor, winterColor, taigaColor, steppeColor };
        
        // Перемешиваем
        for (int i = 0; i < colors.Count; i++) {
            Color temp = colors[i];
            int randomIndex = Random.Range(i, colors.Count);
            colors[i] = colors[randomIndex];
            colors[randomIndex] = temp;
        }

        // Назначаем сторонам света
        northColor = colors[0]; // +Z
        southColor = colors[1]; // -Z
        eastColor = colors[2];  // +X
        westColor = colors[3];  // -X
    }

    void Update()
    {
        if (player == null) return;

        targetColor = DetermineColor(player.position);

        // Плавно меняем цвет материала
        groundMat.color = Color.Lerp(groundMat.color, targetColor, Time.deltaTime * transitionSpeed);
    }

    Color DetermineColor(Vector3 pos)
    {
        // 1. Проверка на центральную зону
        if (Mathf.Abs(pos.x) <= startZoneSize && Mathf.Abs(pos.z) <= startZoneSize)
        {
            return startColor;
        }

        // 2. Определение сектора через сравнение X и Z
        // Это делит мир на 4 бесконечных треугольника, сходящихся в центре
        if (Mathf.Abs(pos.x) > Mathf.Abs(pos.z))
        {
            return pos.x > 0 ? eastColor : westColor;
        }
        else
        {
            return pos.z > 0 ? northColor : southColor;
        }
    }
}
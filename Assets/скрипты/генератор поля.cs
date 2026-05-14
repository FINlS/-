using UnityEngine;

public class BoxGenerator : MonoBehaviour
{
    [Header("Набор плиток")]
    public GameObject[] cellPrefabs; // Массив разных плиток

    [Header("Настройки коробки")]
    public Vector3Int boxSize = new Vector3Int(10, 5, 10);

    public void GenerateHollowBox()
    {
        ClearBox();

        for (int x = 0; x < boxSize.x; x++)
        {
            for (int y = 0; y < boxSize.y; y++)
            {
                for (int z = 0; z < boxSize.z; z++)
                {
                    bool isLeft = (x == 0);
                    bool isRight = (x == boxSize.x - 1);
                    bool isBottom = (y == 0);
                    bool isTop = (y == boxSize.y - 1);
                    bool isFront = (z == 0);
                    bool isBack = (z == boxSize.z - 1);

                    if (isLeft || isRight || isBottom || isTop || isFront || isBack)
                    {
                        if (cellPrefabs == null || cellPrefabs.Length == 0) return;
                        
                        GameObject randomPrefab = cellPrefabs[Random.Range(0, cellPrefabs.Length)];
                        Vector3 pos = new Vector3(x, y, z);
                        GameObject cell = Instantiate(randomPrefab, transform.position + pos, Quaternion.identity, transform);
                        
                        // 1. Сначала ориентируем плитку лицом внутрь
                        Vector3 baseRot = Vector3.zero;
                        if (isLeft) baseRot = new Vector3(0, 90, 0);
                        else if (isRight) baseRot = new Vector3(0, -90, 0);
                        else if (isBottom) baseRot = new Vector3(-90, 0, 0);
                        else if (isTop) baseRot = new Vector3(90, 0, 0);
                        else if (isFront) baseRot = new Vector3(0, 0, 0);
                        else if (isBack) baseRot = new Vector3(0, 180, 0);

                        cell.transform.localEulerAngles = baseRot;

                        // 2. ДОБАВЛЯЕМ СЛУЧАЙНЫЙ ПОВОРОТ вокруг локальной оси Z (0, 90, 180, 270)
                        // Это не изменит направление "взгляда", но "покрутит" саму картинку плитки
                        float[] randomAngles = { 0f, 90f, 180f, 270f };
                        float randomZ = randomAngles[Random.Range(0, randomAngles.Length)];
                        
                        cell.transform.Rotate(Vector3.forward, randomZ, Space.Self);

                        cell.name = $"Cell_{x}_{y}_{z}";
                    }
                }
            }
        }
    }

    public void ClearBox()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
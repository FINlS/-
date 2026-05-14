using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxGenerator))]
public class BoxGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Рисует стандартные поля (размер, префаб)

        BoxGenerator script = (target as BoxGenerator);

        if (GUILayout.Button("Сгенерировать коробку"))
        {
            script.GenerateHollowBox();
        }

        if (GUILayout.Button("Очистить"))
        {
            // Удаляем все дочерние объекты
            while (script.transform.childCount > 0)
            {
                DestroyImmediate(script.transform.GetChild(0).gameObject);
            }
        }
    }
}
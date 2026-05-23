using UnityEngine;

public class FloorOptimizer : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[filters.Length];

        int i = 0;

        foreach (var f in filters)
        {
            combine[i].mesh = f.sharedMesh;
            combine[i].transform = f.transform.localToWorldMatrix;
            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class PieceGenerator : MonoBehaviour
{
    public float length = 200;
    public float width = 200;
    public float heigth = 30;
    private MeshFilter meshFilter;

    Mesh CreateMesh(float length, float width, float heigth)
    {
        int vertices_count = 4 * 6;
        Vector3[] vertices = new Vector3[vertices_count];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, heigth, 0);
        vertices[2] = new Vector3(length, 0, 0);
        vertices[3] = new Vector3(length, heigth, 0);
        vertices[4] = new Vector3(length, 0, width);
        vertices[5] = new Vector3(length, heigth, width);
        vertices[6] = new Vector3(0, 0, width);
        vertices[7] = new Vector3(0, heigth, width);
        vertices[8] = vertices[6];
        vertices[9] = vertices[7];
        vertices[10] = vertices[0];
        vertices[11] = vertices[1];
        vertices[12] = vertices[2];
        vertices[13] = vertices[3];
        vertices[14] = vertices[4];
        vertices[15] = vertices[5];
        vertices[16] = vertices[1];
        vertices[17] = vertices[7];
        vertices[18] = vertices[3];
        vertices[19] = vertices[5];
        vertices[20] = vertices[2];
        vertices[21] = vertices[4];
        vertices[22] = vertices[0];
        vertices[23] = vertices[6];
        int triNum = 6 * 2;
        int triangles_cout = triNum * 3;
        int[] triangles = new int[triangles_cout];
        for (int i = 0, vi = 0; i < triangles_cout; i += 6, vi += 4)
        {
            triangles[i] = vi;
            triangles[i + 1] = vi + 1;
            triangles[i + 2] = vi + 2;
            triangles[i + 3] = vi + 3;
            triangles[i + 4] = vi + 2;
            triangles[i + 5] = vi + 1;
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateMesh(length, width, heigth);
        // AssetDatabase.CreateAsset(meshFilter.mesh, "Assets/Meshes/piece.asset");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

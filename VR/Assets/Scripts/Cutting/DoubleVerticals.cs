using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DoubleVerticals : MonoBehaviour
{


    Mesh mesh;
    Vector3[] origVertices;
    int[] origTrangles;
    // Use this for initialization
    void Start()
    {
        mesh = transform.GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            IncreseVertices();
        }
    }


    void IncreseVertices()
    {

        if (mesh.vertexCount >= 18432)
        {
            Debug.Log("Too Many");
            return;
        }
        origVertices = mesh.vertices;
        origTrangles = mesh.triangles;
        Dictionary<Vector3, int> verticesResultDic = new Dictionary<Vector3, int>();
        List<int> tranglesResultList = new List<int>();
        //计算三角面的个数
        int k = origTrangles.Length / 3;
        int index = 0;
        for (int i = 0; i < k; i++)
        {
            //取出一个三角面（的顶点）
            Vector3[] trangle = new Vector3[3] { origVertices[origTrangles[i * 3]], origVertices[origTrangles[i * 3 + 1]], origVertices[origTrangles[i * 3 + 2]] };

            //通过取三条边的中心点
            //原来三个顶点，变成六个顶点
            Vector3[] result = new Vector3[6];

            Vector3 v01 = (trangle[0] + trangle[1]) * 0.5f;
            Vector3 v12 = (trangle[1] + trangle[2]) * 0.5f;
            Vector3 v02 = (trangle[0] + trangle[2]) * 0.5f;


            if (AddVertices(verticesResultDic, trangle[0], index)) index++;
            if (AddVertices(verticesResultDic, trangle[1], index)) index++;
            if (AddVertices(verticesResultDic, trangle[2], index)) index++;

            if (AddVertices(verticesResultDic, v01, index)) index++;
            if (AddVertices(verticesResultDic, v12, index)) index++;
            if (AddVertices(verticesResultDic, v02, index)) index++;


            // 将原三角面分成新的四个三角面
            // 注意左手法则，逆时针顺序
            //三角形数组存储的是顶点在顶点数组中的序号

            tranglesResultList.Add(verticesResultDic[trangle[0]]);
            tranglesResultList.Add(verticesResultDic[v01]);
            tranglesResultList.Add(verticesResultDic[v02]);

            tranglesResultList.Add(verticesResultDic[v01]);
            tranglesResultList.Add(verticesResultDic[trangle[1]]);
            tranglesResultList.Add(verticesResultDic[v12]);

            tranglesResultList.Add(verticesResultDic[trangle[2]]);
            tranglesResultList.Add(verticesResultDic[v02]);
            tranglesResultList.Add(verticesResultDic[v12]);

            tranglesResultList.Add(verticesResultDic[v02]);
            tranglesResultList.Add(verticesResultDic[v01]);
            tranglesResultList.Add(verticesResultDic[v12]);

        }


        mesh.vertices = GetReusltVertices(verticesResultDic);
        mesh.triangles = tranglesResultList.ToArray();

        mesh.RecalculateBounds();
        //由于normal没有增加，导致表面看起来不平滑(如果要重新计算normals参考顶点的计算)
        mesh.RecalculateNormals();

    }

    bool AddVertices(Dictionary<Vector3, int> verticesResultDic, Vector3 vertice, int index)
    {
        if (verticesResultDic.ContainsValue(index) || verticesResultDic.ContainsKey(vertice))
            return false;

        verticesResultDic.Add(vertice, index);
        return true;
    }

    Vector3[] GetReusltVertices(Dictionary<Vector3, int> verticesResultDic)
    {
        int length = verticesResultDic.Keys.Count;
        Vector3[] result = new Vector3[length];
        List<Vector3> temp = new List<Vector3>(verticesResultDic.Keys);
        for (int i = 0; i < length; i++)
        {
            result[i] = temp[i];
        }

        return result;

    }
}

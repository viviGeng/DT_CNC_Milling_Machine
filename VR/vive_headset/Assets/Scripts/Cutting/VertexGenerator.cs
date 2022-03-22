using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class VertexGenerator : MonoBehaviour
{
    //控制点的大小
    float pointScale = 0.01f;
    public GameObject editorpoint;

    Mesh mesh;

    //顶点列表
    List<Vector3> positionList = new List<Vector3>();
 
    //顶点控制物体列表
    List<GameObject> positionObjList = new List<GameObject>();

    /// <summary>
    /// key:顶点字符串
    /// value:顶点在列表中的位置
    /// </summary>
    Dictionary<string, List<int>> pointmap = new Dictionary<string, List<int>>();


    // 顶点翻倍次数
    int iteration = 5;
    Vector3[] origVertices;
    int[] origTrangles;


    void Awake()
    {
        mesh = transform.GetComponent<MeshFilter>().mesh;
        for (int i = 0; i < iteration; i++)
        {
            IncreseVertices();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        CreateEditorPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //创建控制点
    public void CreateEditorPoint(){
        positionList = new List<Vector3>(mesh.vertices);
 
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (mesh.vertices[i].y != 0.5)
            {
                continue;
            }
            string vstr = Vector2String(mesh.vertices[i]);
 
            if(!pointmap.ContainsKey(vstr)){
                pointmap.Add(vstr,new List<int>());
            }
            pointmap[vstr].Add(i);
        }
 
        foreach (string key in pointmap.Keys)
        {
            editorpoint = Instantiate(editorpoint);
            editorpoint.transform.parent = transform;
            editorpoint.transform.localPosition = String2Vector(key);
            editorpoint.transform.localScale = new Vector3(pointScale, pointScale, pointScale);
 
            MeshEditorPoint editorPoint = editorpoint.GetComponent<MeshEditorPoint>();
            editorPoint.onMove = PointMove;
            editorPoint.pointid = key;
 
            positionObjList.Add(editorpoint);
        }
    }
 
    //顶点物体被移动时调用此方法
    public void PointMove(string pointid,Vector3 position){
        Debug.Log("Here");
        if(!pointmap.ContainsKey(pointid)){
            return;
        }
 
        List<int> _list = pointmap[pointid];
 
        for (int i = 0; i < _list.Count; i ++){
            positionList[_list[i]] = position;
        }
 
        mesh.vertices = positionList.ToArray();
        mesh.RecalculateNormals();
    }

    string Vector2String(Vector3 v){
        StringBuilder str = new StringBuilder();
        str.Append(v.x).Append(",").Append(v.y).Append(",").Append(v.z);
        return str.ToString();
    }
 
    Vector3 String2Vector(string vstr)
    {
        try{
            string[] strings = vstr.Split(',');
            return new Vector3(float.Parse(strings[0]), float.Parse(strings[1]), float.Parse(strings[2]));
        } catch(Exception e){
            Debug.LogError(e.ToString());
            return Vector3.zero;
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
 
    Vector3[] GetReusltVertices( Dictionary< Vector3,int> verticesResultDic)
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

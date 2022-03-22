using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEditorPoint : MonoBehaviour {
 
    //顶点id，（顶点初始位置转字符串）
    [HideInInspector] public string pointid;
 
    //记录坐标点上一次移动的位置，用于判断控制点是否移动
    [HideInInspector] private Vector3 lastPosition;
 
    public delegate void MoveDelegate(string pid,Vector3 pos);
 
    //控制点移动时的回调
    public MoveDelegate onMove = null;

    float height;

    Subscription<HeightEvent> heightSub;
 
	// Use this for initialization
	void Start () {
        lastPosition = transform.position;
        heightSub = EventBus.Subscribe<HeightEvent>(updateHeight);
	}

    void updateHeight(HeightEvent e)
    {
        height = e.height;
    }

	
	// Update is called once per frame
	void Update () {
        if(transform.position != lastPosition){
            if(onMove != null) onMove(pointid, transform.localPosition);
            lastPosition = transform.position;
        }
	}

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Tool")
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
        }
    }

}

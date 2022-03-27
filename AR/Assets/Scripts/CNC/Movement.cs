using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Movement : MonoBehaviour
{
    Client cnc;
    int id;
    float x;
    float y;
    float z;
    bool rLock;
    public GameObject drill;
    Vector3 initial;

    public void ReceiveCoord()
    {
        if(cnc.command.Count() == 0){
            rLock = false;
            return;
        }
        rLock = true;
        JsonData data = cnc.command.Dequeue();
        x = data["x"];
        y = data["y"];
        z = data["z"];
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        Vector3 start = drill.transform.localPosition;
        Vector3 end = initial + new Vector3(-x, -z, y);
        float progress = 0.0f;
        while (progress < 0.1f)
        {
            drill.transform.localPosition = Vector3.Lerp(start, end, progress / 0.1f);
            progress += Time.deltaTime;
            yield return null;
        }
        drill.transform.localPosition = end;
        rLock = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        cnc = new Client();
        cnc.StartThread();
        x = 0.0f;
        y = 0.0f;
        z = 0.0f;
        initial = drill.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (rLock == false)
        {
            ReceiveCoord();
        }
    }
}

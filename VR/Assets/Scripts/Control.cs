using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class Control : MonoBehaviour
{
    Client control;
    float x;
    float y;
    float z;
    float speedX;
    float speedY;
    float speedZ;
    bool mLock;
    bool rLock;
    public GameObject xFrame;
    public GameObject yFrame;
    public GameObject zFrame;
    public bool control;
    public Text info;
    public bool refresh;

    public void Move()
    {
        if(control.command.Count() == 0){
            rLock = false;
            return;
        }
        rLock = true;
        JsonData data = control.command.Dequeue();
        x = data["x"];
        y = data["y"];
        z = data["z"];
        speedX = data["vx"];
        speedY = data["vy"];
        speedZ = data["vz"];
        StartCoroutine(MoveXYZ());
    }

    IEnumerator MoveXYZ()
    {
        while (mLock)
        {
            yield return null;
        }
        mLock = true;
        Vector3 startx = xFrame.transform.position;
        Vector3 starty = yFrame.transform.position;
        Vector3 startz = zFrame.transform.position;
        Vector3 endx = new Vector3(y, xFrame.transform.position.y, xFrame.transform.position.z);
        Vector3 endy = new Vector3(y, yFrame.transform.position.y, -x);
        Vector3 endz = new Vector3(y, z, -x);
        float totalTime = Vector3.Distance(startz, endz) / Mathf.Sqrt(speedX * speedX + speedY * speedY + speedZ * speedZ);
        float progress = 0.0f;
        while (progress < totalTime)
        {
            xFrame.transform.position = Vector3.Lerp(startx, endx, progress / totalTime);
            yFrame.transform.position = Vector3.Lerp(starty, endy, progress / totalTime);
            zFrame.transform.position = Vector3.Lerp(startz, endz, progress / totalTime);
            progress += Time.deltaTime;
            yield return null;
        }
        xFrame.transform.position = endx;
        yFrame.transform.position = endy;
        zFrame.transform.position = endz;
        idLock = false;
        mLock = false;
    }

    public void RemoteControl()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float moveZ = Input.GetAxis("Mouse ScrollWheel");
        xFrame.transform.Translate(moveY, 0.0f, 0.0f);
        yFrame.transform.Translate(moveY, 0.0f, -moveX);
        zFrame.transform.Translate(moveY, 3 * moveZ, -moveX);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JsonData data = new JsonData();
            data["x"] = -zFrame.transform.position.z;
            data["y"] = -zFrame.transform.position.x;
            data["z"] = -zFrame.transform.position.y;
            data["sender"] = "VR";
            control.SendMessage(data);
        }
    }

    public void ResetToOrigin()
    {
        control.command.Clear();
        xFrame.transform.position = Vector3.zero;
        yFrame.transform.position = Vector3.zero;
        zFrame.transform.position = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        control = new Client();
        control.StartThread();
        x = 0.0f;
        y = 0.0f;
        z = 0.0f;
        speedX = 0.0f;
        speedY = 0.0f;
        speedZ = 0.0f;
        mLock = false;
        rLock = false;
        control = false;
        refresh = false;
    }

    // Update is called once per frame
    void Update()
    {
        info.text = string.Format("x: {0}\ny: {1}\nz: {2}", -zFrame.transform.position.z, -zFrame.transform.position.x, -zFrame.transform.position.y);
        if (control == true)
        {
            RemoteControl();
        }
        if (control == false && moveLock == false && refresh == true)
        {
            Move();
        }
    }
}

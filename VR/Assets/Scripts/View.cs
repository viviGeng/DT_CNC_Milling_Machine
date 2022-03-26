using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    public Transform target;
    private int sensitivity = 100;
    private int zoomMin = 10;
    private int zoomMax = 1500;
    private float moveSpeed = 1.0f;
    private float rotateSpeedX = 250.0f;
    private float rotateSpeedY = 250.0f;
    private int rotateMinY = -360;
    private int rotateMaxY = 360;
    private float eulerX = 0.0f;
    private float eulerY = 0.0f;
    private float Distance = 500.0f;
    private Vector3 targetOnScreenPosition;
    private Quaternion storeRotation;
    private Vector3 viewTargetPosition;
    private Vector3 initPosition;
    private Vector3 cameraX;
    private Vector3 cameraY;
    private Vector3 cameraZ;
    private Vector3 initScreenPos;
    private Vector3 curScreenPos;
    GameObject manager;

    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    // Start is called before the first frame update
    void Start()
    {
        eulerX = transform.eulerAngles.y;
        eulerY = transform.eulerAngles.x;
        viewTargetPosition = target.position;
        storeRotation = Quaternion.Euler(eulerY + 60, eulerX, 0);
        transform.rotation = storeRotation;
        Vector3 position = storeRotation * new Vector3(0.0F, 0.0F, -Distance) + viewTargetPosition;
        transform.position = storeRotation * new Vector3(0, 0, -Distance) + viewTargetPosition;
        manager = GameObject.Find("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        // if (manager.GetComponent<Control>().control == true)
        // {
        //     return;
        // }
        if (Input.GetMouseButton(1))
        {
            eulerX += Input.GetAxis("Mouse X") * rotateSpeedX * 0.02f;
            eulerY -= Input.GetAxis("Mouse Y") * rotateSpeedY * 0.02f;
            eulerY = ClampAngle(eulerY, rotateMinY, rotateMaxY);
            storeRotation = Quaternion.Euler(eulerY + 60.0f, eulerX, 0.0f);
            transform.rotation = storeRotation;
            transform.position = storeRotation * new Vector3(0.0f, 0.0f, -Distance) + viewTargetPosition; ;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0 &&  Input.GetKey(KeyCode.Z))
        {
            if (Distance >= zoomMin && Distance <= zoomMax)
            {
                Distance -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            }
            if (Distance < zoomMin)
            {
                Distance = zoomMin;
            }
            if (Distance > zoomMax)
            {
                Distance = zoomMax;
            }
            transform.position = storeRotation * new Vector3(0.0F, 0.0F, -Distance) + viewTargetPosition;
        }
        if (Input.GetMouseButtonDown(0))
        {
            cameraX = transform.right;
            cameraY = transform.up;
            cameraZ = transform.forward;
            initScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetOnScreenPosition.z);
            targetOnScreenPosition = Camera.main.WorldToScreenPoint(viewTargetPosition);
            initPosition = viewTargetPosition;
        }
        if (Input.GetMouseButton(0))
        {
            curScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, targetOnScreenPosition.z);
            target.position = initPosition - moveSpeed * ((curScreenPos.x - initScreenPos.x) * cameraX + (curScreenPos.y - initScreenPos.y) * cameraY);
            transform.position = storeRotation * new Vector3(0.0F, 0.0F, -Distance) + target.position; ;
        }
        if (Input.GetMouseButtonUp(0))
        {
            viewTargetPosition = target.position;
        }
    }
}

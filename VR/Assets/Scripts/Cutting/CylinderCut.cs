using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderCut : MonoBehaviour
{
    public float moveSpeed = 2.0f;

    Vector3 centerOffset;
    public float height;

    // Start is called before the first frame update
    void Start()
    {
        centerOffset = GetComponent<CapsuleCollider>().center;
        height = GetComponent<CapsuleCollider>().height * gameObject.transform.localScale.y / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // float MoveH = Input.GetAxis("Horizontal");
        // float MoveV = Input.GetAxis("Vertical");
        // float MoveZ = Input.GetAxis("Mouse ScrollWheel") * 3.0f;
        // transform.Translate(new Vector3(MoveH, MoveZ, MoveV) * Time.deltaTime * moveSpeed);

        EventBus.Publish<HeightEvent>(new HeightEvent(gameObject.transform.position.y - height));

    }
}

public class HeightEvent
{
    public float height;
    public HeightEvent(float _height)
    {
        height = _height;
    }
}
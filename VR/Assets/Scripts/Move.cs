using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float moveSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float MoveH = Input.GetAxis("Horizontal");
        float MoveV = Input.GetAxis("Vertical");
        float MoveZ = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(new Vector3(MoveH, MoveZ, MoveV) * Time.deltaTime * moveSpeed);
    }
}

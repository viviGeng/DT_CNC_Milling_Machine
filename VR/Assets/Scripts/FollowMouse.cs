using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    GameObject spindle;

    // Start is called before the first frame update
    void Start()
    {
        spindle = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) == true)
            {
                spindle = hit.collider.gameObject;
            }
        }
        if (spindle != null)
        {
            spindle.transform.position = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}

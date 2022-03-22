using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class Piece : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     Debug.Log("collide");
    //     CSG_Model result = Boolean.Subtract(gameObject, other.gameObject);
    //     GetComponent<MeshFilter>().mesh = result.mesh;
    // }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("collide");
        CSG_Model result = Boolean.Subtract(gameObject, other.gameObject);
        GetComponent<MeshFilter>().mesh = result.mesh;
    }
}

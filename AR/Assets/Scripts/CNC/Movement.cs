using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class Movement : MonoBehaviour
{
    MySqlConnection mySqlConnection;
    int id;
    float x;
    float y;
    float z;
    bool idLock;
    public GameObject drill;
    Vector3 initial;

    public void ReceiveCoord()
    {
        idLock = true;
        string query = string.Format("select x,y,z from positions where id={0}", id);
        MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        if (rdr.Read())
        {
            id += 1;
            x = rdr.GetFloat("x");
            y = rdr.GetFloat("y");
            z = rdr.GetFloat("z");
            StartCoroutine(Move());
        }
        else
        {
            idLock = false;
        }
        rdr.Close();
        rdr.Dispose();
        cmd.Dispose();
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
        idLock = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = Connection.mySqlConnection;
        id = 1;
        x = 0.0f;
        y = 0.0f;
        z = 0.0f;
        initial = drill.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(drill.transform.localPosition);
        if (idLock == false)
        {
            ReceiveCoord();
        }
    }
}

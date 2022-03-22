using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    MySqlConnection mySqlConnection;
    int id;
    float x;
    float y;
    float z;
    float speedX;
    float speedY;
    float speedZ;
    bool mLock;
    bool idLock;
    public GameObject xFrame;
    public GameObject yFrame;
    public GameObject zFrame;
    public bool control;
    public Text info;
    public bool refresh;

    public void Move()
    {
        idLock = true;
        // id += 1;
        string query = string.Format("select * from positions where id={0}", id);
        MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        if (rdr.Read())
        {
            id += 1;
            x = rdr.GetFloat("x");
            y = rdr.GetFloat("y");
            z = rdr.GetFloat("z");
            speedX = rdr.GetFloat("vx");
            speedY = rdr.GetFloat("vy");
            speedZ = rdr.GetFloat("vz");
            StartCoroutine(MoveXYZ());
        }
        else
        {
            idLock = false;
        }
        rdr.Close();
        rdr.Dispose();
        cmd.Dispose();
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
            string query = string.Format("insert into feedback (x,y,z) values ({0},{1},{2});", -zFrame.transform.position.z, -zFrame.transform.position.x, -zFrame.transform.position.y);
            MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }

    public void ResetToOrigin()
    {
        string resetPositions = string.Format("truncate table positions;");
        MySqlCommand resetPositionsCmd = new MySqlCommand(resetPositions, mySqlConnection); ;
        resetPositionsCmd.ExecuteNonQuery();
        resetPositionsCmd.Dispose();
        id = 1;
        xFrame.transform.position = Vector3.zero;
        yFrame.transform.position = Vector3.zero;
        zFrame.transform.position = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = ConnectionMySQL.mySqlConnection;
        id = 1;
        x = 0.0f;
        y = 0.0f;
        z = 0.0f;
        speedX = 0.0f;
        speedY = 0.0f;
        speedZ = 0.0f;
        mLock = false;
        idLock = false;
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
        if (control == false && idLock == false && refresh == true)
        {
            Move();
        }
    }
}

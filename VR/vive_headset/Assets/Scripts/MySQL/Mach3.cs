using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine.UI;

public class Mach3 : MonoBehaviour
{
    MySqlConnection mySqlConnection;
    string filePath;
    StreamReader sr;
    string parameters;
    public Image indicator;
    public Text path;
    // public GameObject button;
    static Color32[] colors = { new Color32(95, 87, 82, 255), new Color32(0, 150, 87, 255) };

    public void SetPath()
    {
        if (System.IO.File.Exists(path.text))
        {
            sr = File.OpenText(path.text);
            filePath = path.text;
            Debug.Log("Valid path");
        }
        else
        {
            // filePath = null;
            Debug.Log("Invalid path");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = ConnectionMySQL.mySqlConnection;
    }

    // Update is called once per frame
    void Update()
    {
        indicator.GetComponent<Image>().color = colors[mySqlConnection.State.ToString() == "Open" ? 1 : 0];
        // if (filePath != null)
        // {
        //     Debug.Log("File assigned");
        // }
        // else
        // {
        //     Debug.Log("File not assigned");
        // }
        if (filePath != null)
        {
            if (sr.Peek() >= 0 && (parameters = sr.ReadLine()) != null)
            {
                Debug.Log(parameters);
                string[] xyza = parameters.Split(' ');
                string query = string.Format("insert into positions (x,y,z,a) values ({0},{1},{2},{3});", xyza[0], xyza[1], xyza[2], xyza[3]);
                Debug.Log(query);
                MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }
    }
}

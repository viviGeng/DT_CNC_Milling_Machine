using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine.UI;

public class Mach3V1 : MonoBehaviour
{
    MySqlConnection mySqlConnection;
    string filePath;
    string gcodePath;
    StreamReader sr;
    string parameters;
    public Image indicator;
    public Text gPath;
    public Text fPath;
    Coroutine uploadProcess;
    static Color32[] colors = { new Color32(95, 87, 82, 255), new Color32(0, 150, 87, 255) };
    static string getCoordinate = "G02 X0. Y0.5 I0.5 J0. F2.5\nX0.5 Y0. I0. J-0.5\nX0. Y-0.5 I-0.5 J0.\nX-0.5 Y0. I0. J0.5\nG01 Z0.1 F5.\nG00 X0. Y0. Z0.25";

    public void SetPath()
    {
        if (uploadProcess != null)
        {
            StopCoroutine(uploadProcess);
        }
        if (sr != null)
        {
            sr.Close();
            sr.Dispose();
        }
        if (System.IO.Directory.Exists(Path.GetDirectoryName(gPath.text)))
        {
            if (System.IO.File.Exists(gPath.text))
            {
                System.IO.File.Delete(gPath.text);
            }
            System.IO.File.WriteAllText(gPath.text, getCoordinate);
            gcodePath = gPath.text;
            Debug.Log("Valid gcode path");
        }
        else
        {
            Debug.Log("Invalid gcode path");
            return;
        }
        if (System.IO.Directory.Exists(Path.GetDirectoryName(fPath.text)))
        {
            if (System.IO.File.Exists(fPath.text))
            {
                System.IO.File.Delete(fPath.text);
            }
            System.IO.File.WriteAllText(fPath.text, null);
            filePath = fPath.text;
            Debug.Log("Valid file path");
        }
        else
        {
            Debug.Log("Invalid file path");
            return;
        }
        uploadProcess = StartCoroutine(UploadData());
    }

    IEnumerator UploadData()
    {
        while (true)
        {
            sr = File.OpenText(fPath.text);
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
            sr.Close();
            sr.Dispose();
            System.IO.File.Delete(filePath);
            System.IO.File.WriteAllText(filePath, null);
            yield return new WaitForSeconds(2);
            System.IO.File.Delete(gcodePath);
            System.IO.File.WriteAllText(gcodePath, getCoordinate);
            yield return new WaitForSeconds(2);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = ConnectionMySQL.mySqlConnection;
        // uploadProcess = StartCoroutine(UploadData());
    }

    // Update is called once per frame
    void Update()
    {
        indicator.GetComponent<Image>().color = colors[mySqlConnection.State.ToString() == "Open" ? 1 : 0];
        // if (gcodePath != null && filePath != null)
        // {
        //     if (sr.Peek() >= 0 && (parameters = sr.ReadLine()) != null)
        //     {
        //         Debug.Log(parameters);
        //         string[] xyza = parameters.Split(' ');
        //         string query = string.Format("insert into positions (x,y,z,a) values ({0},{1},{2},{3});", xyza[0], xyza[1], xyza[2], xyza[3]);
        //         Debug.Log(query);
        //         MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
        //         cmd.ExecuteNonQuery();
        //         cmd.Dispose();
        //     }
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine.UI;
using System.Net.Sockets;
using System;

public class Mach3V3 : MonoBehaviour
{
    MySqlConnection mySqlConnection;
    string commandPath;
    string outputPath;
    StreamReader sr;
    string parameters;
    public Image indicator;
    public Text cPath;
    public Text oPath;
    Coroutine commandProcess;
    static Color32[] colors = { new Color32(95, 87, 82, 255), new Color32(0, 150, 87, 255) };
    static string[] commands = { "load", "start", "stop", "pause", "continue", "record" };
    static string[] results = { "record.txt" };
    float delay = 0.1f;
    string commandStr;
    int commandId;
    bool idLock;
    TcpClient commandSocket;
    NetworkStream commandStream;
    StreamWriter commandWriter;
    string host = "localhost";
    Int32 port = 8080;
    internal Boolean socketReady = false;

    public void SetPath()
    {
        StopProcess();
        if (System.IO.Directory.Exists(cPath.text))
        {
            commandPath = cPath.text;
            Debug.Log("Valid command path");
        }
        else
        {
            Debug.Log("Invalid command path");
            return;
        }
        if (System.IO.Directory.Exists(oPath.text))
        {
            outputPath = oPath.text;
            Debug.Log("Valid output path");
        }
        else
        {
            Debug.Log("Invalid output path");
            return;
        }
        SendCommand(commands[0]);
    }

    IEnumerator Process()
    {
        while (true)
        {
            SendCommand(commands[5]);
            yield return new WaitForSeconds(delay);
            if (System.IO.File.Exists(Path.Combine(outputPath, results[0])) == false)
            {
                Debug.Log("No result");
                yield return new WaitForSeconds(delay);
                continue;
            }
            sr = File.OpenText(Path.Combine(outputPath, results[0]));
            if ((parameters = sr.ReadLine()) != null)
            {
                Debug.Log(parameters);
                string[] xyzv = parameters.Split(' ');
                string query = string.Format("insert into positions (x,y,z,vx,vy,vz) values ({0},{1},{2},{3},{4},{5});", xyzv[0], xyzv[1], xyzv[2], xyzv[3], xyzv[4], xyzv[5]);
                Debug.Log(query);
                MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            sr.Close();
            sr.Dispose();
            yield return new WaitForSeconds(delay);
        }
    }

    public void StopProcess()
    {
        if (commandProcess != null)
        {
            StopCoroutine(commandProcess);
        }
        if (sr != null)
        {
            sr.Close();
            sr.Dispose();
        }
        if (commandPath == null)
        {
            return;
        }
    }

    public void ReceiveCommands()
    {
        idLock = true;
        string query = string.Format("select command from commands where id={0}", commandId);
        MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        if (rdr.Read())
        {
            commandId += 1;
            commandStr = rdr.GetString("command");
            Debug.Log(commandStr);
            switch (commandStr)
            {
                case "STOP":
                    StopProcess();
                    SendCommand(commands[2]);
                    break;
                case "START":
                    SendCommand(commands[1]);
                    commandProcess = StartCoroutine(Process());
                    break;
                case "PAUSE":
                    SendCommand(commands[3]);
                    break;
                case "CONTINUE":
                    SendCommand(commands[4]);
                    break;
                // case "LOAD":
                //     System.IO.File.WriteAllText(Path.Combine(commandPath, commands[0]), null);
                //     break;
                default:
                    break;
            }
        }
        rdr.Close();
        rdr.Dispose();
        cmd.Dispose();
        idLock = false;
    }

    void SendCommand(string command)
    {
        if (socketReady == false)
        {
            return;
        }
        commandWriter.Write(command);
        commandWriter.Flush();
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = ConnectionMySQL.mySqlConnection;
        try
        {
            commandSocket = new TcpClient(host, port);
            commandStream = commandSocket.GetStream();
            commandWriter = new StreamWriter(commandStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
        commandId = 1;
        idLock = false;
    }

    // Update is called once per frame
    void Update()
    {
        indicator.GetComponent<Image>().color = colors[mySqlConnection.State.ToString() == "Open" ? 1 : 0];
        if (idLock == false)
        {
            ReceiveCommands();
        }
    }
}

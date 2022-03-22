using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine.UI;
using System.Net.Sockets;
using System;

public class Mach3V2 : MonoBehaviour
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
    // static string[] commands = { "load.sig", "start.sig", "stop.sig", "pause.sig", "continue.sig", "record.sig", "reset.sig", "control.sig", "simulate.sig" };
    static string[] commands = { "load", "start", "stop", "pause", "continue", "record", "reset", "control", "simulate" };
    static string[] results = { "record.txt" };
    float delay = 0.5f;
    string commandStr;
    int commandId;
    bool idLock;
    TcpClient commandSocket;
    NetworkStream commandStream;
    StreamWriter commandWriter;
    string host = "localhost";
    Int32 port = 8080;
    internal Boolean socketReady = false;
    int coordId;
    bool coordLock;
    bool control;
    public Text message;

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
        // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[0]), null);
        // SendCommand(commands[0]);
        // commandProcess = StartCoroutine(Process());
    }

    IEnumerator Process()
    {
        while (true)
        {
            // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[5]), null);
            SendCommand(commands[5]);
            yield return new WaitForSeconds(delay);
            // System.IO.File.Delete(Path.Combine(commandPath, commands[5]));
            // yield return new WaitForSeconds(delay);
            if (System.IO.File.Exists(Path.Combine(outputPath, results[0])) == false)
            {
                Debug.Log("No result");
                yield return new WaitForSeconds(delay);
                continue;
            }
            sr = File.OpenText(Path.Combine(outputPath, results[0]));
            Debug.LogWarning("Here");
            if ((parameters = sr.ReadLine()) != null)
            {
                Debug.LogWarning(parameters);
                string[] xyzv = parameters.Split(' ');
                string query = string.Format("insert into positions (x,y,z,vx,vy,vz) values ({0},{1},{2},{3},{4},{5});", xyzv[0], xyzv[1], xyzv[2], xyzv[3], xyzv[4], xyzv[5]);
                Debug.Log(query);
                MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            sr.Close();
            sr.Dispose();
            // System.IO.File.Delete(Path.Combine(commandPath, commands[5]));
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
        // foreach (string command in commands)
        // {
        //     if (System.IO.File.Exists(Path.Combine(commandPath, command)))
        //     {
        //         System.IO.File.Delete(Path.Combine(commandPath, command));
        //     }
        // }
    }

    public void ResetProcess()
    {
        // commandPath = null;
        // outputPath = null;
        // string reset;
        // MySqlCommand resetCmd;
        // reset = string.Format("truncate table commands;");
        // resetCmd = new MySqlCommand(reset, mySqlConnection);
        // resetCmd.ExecuteNonQuery();
        // resetCmd.Dispose();
        // commandId = 1;
        // reset = string.Format("truncate table feedback;");
        // resetCmd = new MySqlCommand(reset, mySqlConnection);
        // resetCmd.ExecuteNonQuery();
        // resetCmd.Dispose();
        // coordId = 1;
    }

    public void ResetDatabase()
    {
        // string resetPositions = string.Format("truncate table positions;");
        // MySqlCommand resetPositionsCmd = new MySqlCommand(resetPositions, mySqlConnection); ;
        // resetPositionsCmd.ExecuteNonQuery();
        // resetPositionsCmd.Dispose();
        // string resetCommands = string.Format("truncate table commands;");
        // MySqlCommand resetCommandsCmd = new MySqlCommand(resetCommands, mySqlConnection); ;
        // resetCommandsCmd.ExecuteNonQuery();
        // resetCommandsCmd.Dispose();
        // string resetFeedback = string.Format("truncate table feedback;");
        // MySqlCommand resetFeedbackCmd = new MySqlCommand(resetFeedback, mySqlConnection); ;
        // resetFeedbackCmd.ExecuteNonQuery();
        // resetFeedbackCmd.Dispose();
        // commandId = 1;
        // coordId = 1;
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
                    // *
                    // ResetDatabase();
                    // *
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[2]), null);
                    SendCommand(commands[2]);
                    break;
                case "START":
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[1]), null);
                    SendCommand(commands[1]);
                    commandProcess = StartCoroutine(Process());
                    break;
                case "PAUSE":
                    StopProcess();
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[3]), null);
                    SendCommand(commands[3]);
                    break;
                case "CONTINUE":
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[4]), null);
                    SendCommand(commands[4]);
                    commandProcess = StartCoroutine(Process());
                    break;
                case "LOAD":
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[0]), null);
                    SendCommand(commands[0]);
                    break;
                case "RESET":
                    ResetProcess();
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[6]), null);
                    SendCommand(commands[6]);
                    break;
                case "CONTROL":
                    StopProcess();
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[7]), null);
                    SendCommand(commands[7]);
                    control = true;
                    break;
                case "SIMULATE":
                    control = false;
                    // System.IO.File.WriteAllText(Path.Combine(commandPath, commands[8]), null);
                    SendCommand(commands[8]);
                    commandProcess = StartCoroutine(Process());
                    break;
                default:
                    break;
            }
        }
        rdr.Close();
        rdr.Dispose();
        cmd.Dispose();
        idLock = false;
    }

    public void ReceiveCoords()
    {
        coordLock = true;
        // id += 1;
        string query = string.Format("select * from feedback where id={0}", coordId);
        MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        if (rdr.Read())
        {
            coordId += 1;
            float x = rdr.GetFloat("x");
            float y = rdr.GetFloat("y");
            float z = rdr.GetFloat("z");
            string gcode = string.Format("G01 X{0} Y{1} Z{2} F200", x, y, z);
            SendCommand(gcode);
        }
        rdr.Close();
        rdr.Dispose();
        cmd.Dispose();
        coordLock = false;
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

    void ResetDB(string table)
    {
        string resetDB = string.Format("truncate table {0};", table);
        MySqlCommand resetDBCmd = new MySqlCommand(resetDB, mySqlConnection);
        resetDBCmd.ExecuteNonQuery();
        resetDBCmd.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = ConnectionMySQL.mySqlConnection;
        ResetDB("commands");
        ResetDB("feedback");
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
        coordId = 1;
        coordLock = false;
        control = false;
    }

    // Update is called once per frame
    void Update()
    {
        indicator.GetComponent<Image>().color = colors[mySqlConnection.State.ToString() == "Open" ? 1 : 0];
        if (idLock == false)
        {
            ReceiveCommands();
        }
        if (control == true && coordLock == false)
        {
            ReceiveCoords();
        }
    }
}

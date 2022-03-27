using System.Collections;
using System.Collections.Generic;
using System.Timers.Time;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using LitJson;

public class Mach3V3 : MonoBehaviour
{
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
    Client mach;
    System.Timers.Timer timer;
    StreamWriter commandWriter;
    TcpClient commandSocket;
    NetworkStream commandStream;
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
                JsonData data = new JsonData();
                data["x"] = xyzv[0];
                data["y"] = xyzv[1];
                data["z"] = xyzv[2];
                data["vx"] = xyzv[3];
                data["vy"] = xyzv[4];
                data["vz"] = xyzv[5];
                data["sender"] = "Mach3";
                data["note"] = "transmit";
                mach.SendMessage(data);
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
        if(mach.command.Count() == 0){
            return;
        }
        JsonData data = mach.command.Dequeue();
        commandStr = data["command"];
        switch(commandStr){
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
            default:
                break;
        }
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
        try
        {
            mach = new Client();
            mach.StartThread();
            commandSocket = new TcpClient(host, port);
            commandStream = commandSocket.GetStream();
            commandWriter = new StreamWriter(commandStream);
            socketReady = true;
            JsonData data = new JsonData();
            data["sender"] = "Mach3";
            data["note"] = "connect";
            mach.SendMessage(data);
            timer = new System.Timers.Timer();
            timer.Interval = 500;
            timer.Elapsed += delegate{
                JsonData data = new JsonData();
                data["sender"] = "Mach3";
                data["note"] = "heartbeat";
                mach.SendMessage(data);
            };
            timer.Start();
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
        // commandId = 1;
        // idLock = false;
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveCommands();
    }
}

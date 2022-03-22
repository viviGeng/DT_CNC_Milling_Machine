using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class ControlButton : MonoBehaviour
{
    MySqlConnection mySqlConnection;
    public Text pauseContinue;
    public Text stopStart;
    public Text loadReset;
    public Text controlSimulate;
    public Text buttonInfo;
    GameObject manager;

    public void SendCommand()
    {
        if (stopStart.text == "START" && (buttonInfo.text == "PAUSE" || buttonInfo.text == "CONTINUE"))
        {
            return;
        }
        if (stopStart.text == "STOP" && (buttonInfo.text == "LOAD" || buttonInfo.text == "RESET"))
        {
            return;
        }
        if (buttonInfo.text == "START" && loadReset.text == "LOAD")
        {
            return;
        }
        if (buttonInfo.text == "STOP" && pauseContinue.text == "CONTINUE")
        {
            return;
        }
        if (buttonInfo.text == "CONTROL")
        {
            manager.GetComponent<Control>().control = true;
            // string q = string.Format("truncate table positions;");
            // MySqlCommand c = new MySqlCommand(q, mySqlConnection);
            // c.ExecuteNonQuery();
            // c.Dispose();
            // UpdateCommand();
            // return;
        }
        if (buttonInfo.text == "SIMULATE")
        {
            manager.GetComponent<Control>().control = false;
            // UpdateCommand();
            // return;
        }
        // *
        if (buttonInfo.text == "START")
        {
            manager.GetComponent<Control>().ResetToOrigin();
            manager.GetComponent<Control>().refresh = true;
        }
        if (buttonInfo.text == "STOP")
        {
            manager.GetComponent<Control>().refresh = false;
            // manager.GetComponent<Control>().BacktoOrigin();
            // manager.GetComponent<Control>().ResetId();
        }
        if (buttonInfo.text == "CONTINUE")
        {
            manager.GetComponent<Control>().refresh = true;
        }
        if (buttonInfo.text == "PAUSE")
        {
            manager.GetComponent<Control>().refresh = false;
        }
        // *
        // if (buttonInfo.text == "RESET")
        // {
        //     string reset = string.Format("truncate table positions;");
        //     MySqlCommand resetCmd = new MySqlCommand(reset, mySqlConnection);
        //     resetCmd.ExecuteNonQuery();
        //     resetCmd.Dispose();
        //     manager.GetComponent<Control>().ResetId();
        // }
        string query = string.Format("insert into commands (command) values ('{0}');", buttonInfo.text);
        Debug.Log(query);
        MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
        cmd.ExecuteNonQuery();
        cmd.Dispose();
        UpdateCommand();
    }

    public void UpdateCommand()
    {
        switch (buttonInfo.text)
        {
            case "PAUSE":
                buttonInfo.text = "CONTINUE";
                break;
            case "CONTINUE":
                buttonInfo.text = "PAUSE";
                break;
            case "STOP":
                buttonInfo.text = "START";
                break;
            case "START":
                buttonInfo.text = "STOP";
                pauseContinue.text = "PAUSE";
                break;
            case "LOAD":
                buttonInfo.text = "RESET";
                break;
            case "RESET":
                buttonInfo.text = "LOAD";
                break;
            case "CONTROL":
                buttonInfo.text = "SIMULATE";
                break;
            case "SIMULATE":
                buttonInfo.text = "CONTROL";
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = ConnectionMySQL.mySqlConnection;
        manager = GameObject.Find("Manager");
    }

    // Update is called once per frame
    void Update()
    {

    }
}

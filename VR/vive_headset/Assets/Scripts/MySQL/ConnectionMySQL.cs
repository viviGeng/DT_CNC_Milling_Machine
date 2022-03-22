using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class ConnectionMySQL : MonoBehaviour
{
    public static MySqlConnection mySqlConnection;
    // public static bool connected = false;
    // static string host = "127.0.0.1";
    // static string port = "3306";
    // static string user = "root";
    // static string pwd = "Demian1973";
    // static string db = "450";
    static string host = "35.225.65.175";
    static string user = "zhiqich";
    static string pwd = "Demian1973";
    static string db = "450";

    public static void Open()
    {
        try
        {
            // string connectionString = string.Format("server={0};port={1};user={2};password={3};database={4};", host, port, user, pwd, db);
            string connectionString = string.Format("Host={0};Uid={1};Pwd={2};Database={3};", host, user, pwd, db);
            mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            // connected = true;
            Debug.Log("Connected");
        }
        catch (MySqlException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public static void Close()
    {
        if (mySqlConnection != null)
        {
            mySqlConnection.Close();
            mySqlConnection.Dispose();
            mySqlConnection = null;
            // connected = false;
            Debug.Log("Disconnected");
        }
    }

    private void Awake()
    {
        Open();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(mySqlConnection.State);
    }
}

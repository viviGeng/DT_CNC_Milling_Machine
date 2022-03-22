using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;

public class ConnectionPostgreSQL : MonoBehaviour
{
    public static NpgsqlConnection postgreSqlConnection;
    static string host = "34.68.251.249";
    static string user = "zhiqich";
    static string pwd = "Demian1973";
    static string db = "450";

    public static void Open()
    {
        try
        {
            string connectionString = string.Format("sslmode=disable;server={0};user={1};password={2};database={3};", host, user, pwd, db);
            postgreSqlConnection = new NpgsqlConnection(connectionString);
            postgreSqlConnection.Open();
            Debug.Log("Connected");
        }
        catch (NpgsqlException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public static void Close()
    {
        if (postgreSqlConnection != null)
        {
            postgreSqlConnection.Close();
            postgreSqlConnection.Dispose();
            postgreSqlConnection = null;
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

    }
}

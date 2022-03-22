using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    MySqlConnection mySqlConnection;
    public Text userFeedback;
    string userId;
    string userName;

    public void GetUser(int id)
    {
        string query = string.Format("select * from users where id={0}", id);
        MySqlCommand cmd = new MySqlCommand(query, mySqlConnection);
        MySqlDataReader rdr = cmd.ExecuteReader();
        if (rdr.Read())
        {
            userId = rdr.GetString("id");
            userName = rdr.GetString("name");
            userFeedback.text = userId + " " + userName;
        }
        else
        {
            Debug.Log("User not exist");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mySqlConnection = ConnectionMySQL.mySqlConnection;
        GetUser(1);
        ConnectionMySQL.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

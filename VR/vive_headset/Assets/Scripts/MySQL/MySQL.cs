using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using MySql.Data.MySqlClient;
// using UnityEngine.UI;

public class MySQL : MonoBehaviour
{
    // public Text feedback;
    // string id;
    // string name;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     string connetStr = "server=localhost;port=3306;user=root;password=Demian1973;database=450;SslMode=none;";
    //     MySqlConnection conn = new MySqlConnection(connetStr);
    //     try
    //     {
    //         conn.Open();
    //         Debug.Log("success");
    //         string query = "select * from users";
    //         MySqlCommand cmd = new MySqlCommand(query, conn);
    //         MySqlDataReader rdr = cmd.ExecuteReader();
    //         if (rdr.Read())
    //         {
    //             id = rdr.GetString("id");
    //             name = rdr.GetString("name");
    //             feedback.text = id + " " + name;
    //         }
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Debug.Log(ex.Message);
    //     }
    //     finally
    //     {
    //         conn.Close();
    //     }
    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}

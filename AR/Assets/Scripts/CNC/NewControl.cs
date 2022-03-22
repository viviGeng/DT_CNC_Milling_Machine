using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NewControl : MonoBehaviour
{
    public GameObject drill;
    Vector3 initial;
    int id;
    float x;
    float y;
    float z;
    bool idLock;

    IEnumerator GetRequest(string uri)
    {
        idLock = true;
        // Debug.Log(count++);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            if (webRequest.isNetworkError)
            {
                Debug.Log("Fail!");
                idLock = false;
            }
            else
            {
                // Debug.Log("Success!");
                if (webRequest.downloadHandler.text != "***")
                {
                    string[] coord = webRequest.downloadHandler.text.Split(',');
                    Debug.Log(coord);
                    x = float.Parse(coord[0]);
                    y = float.Parse(coord[1]);
                    z = float.Parse(coord[2]);
                    id += 1;
                    StartCoroutine(Move());
                }
                else
                {
                    idLock = false;
                }
            }
        }
        // idLock = false;
    }

    IEnumerator Move()
    {
        Vector3 start = drill.transform.localPosition;
        Vector3 end = initial + new Vector3(-x, -z, y);
        float progress = 0.0f;
        while (progress < 0.1f)
        {
            drill.transform.localPosition = Vector3.Lerp(start, end, progress / 0.1f);
            progress += Time.deltaTime;
            yield return null;
        }
        drill.transform.localPosition = end;
        idLock = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        id = 1;
        x = 0.0f;
        y = 0.0f;
        z = 0.0f;
        initial = drill.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (idLock == false)
        {
            StartCoroutine(GetRequest("https://phpmyadmin-dot-graphic-outlook-279320.appspot.com/get_positions.php?id=" + id.ToString()));
        }
    }
}

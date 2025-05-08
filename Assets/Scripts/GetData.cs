using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;


public class GetData : MonoBehaviour
{
    public string DataURL;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getData());
    }

    IEnumerator getData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(DataURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log(json);
                ReadJSON(json);
            }
        }
    }

    void ReadJSON(string jsonString)
    {
        JSONNode node = JSON.Parse(jsonString);
        JSONObject obj = node.AsObject;
        Debug.Log(obj["gaza"]["last_update"].Value);            //Summary report-date
        Debug.Log(obj["gaza"]["killed"]["children"].Value);     //Summary total child deaths in Gaza

        
        Debug.Log(obj["west_bank"]["last_update"].Value);            //Summary report-date
        Debug.Log(obj["west_bank"]["killed"]["children"].Value);     //Summary total child deaths in West-Bank

    }
}
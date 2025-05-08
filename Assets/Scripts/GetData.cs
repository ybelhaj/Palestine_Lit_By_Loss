using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;

public class GetData : MonoBehaviour
{
    public string DataURL;

    void Start()
    {
        StartCoroutine(GetDataFromWeb());
    }

    IEnumerator GetDataFromWeb()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(DataURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                ReadJSON(json);
            }
        }
    }

    void ReadJSON(string jsonString)
    {
        JSONNode node = JSON.Parse(jsonString);
        GetDataHelper.StoreData(node);
    }
}

// --- Helper class (not nested!) ---
public static class GetDataHelper
{
    public static Dictionary<string, JSONNode> RegionData = new();

    public static void StoreData(JSONNode node)
    {
        RegionData["gaza"] = node["gaza"];
        RegionData["west_bank"] = node["west_bank"];
    }

    public static string GetFormattedInfo(string regionKey)
    {
        if (!RegionData.ContainsKey(regionKey)) return "Data not found.";

        var data = RegionData[regionKey];
        string regionName = regionKey.Replace("_", " ").ToUpper();

        return $"Region: {regionName}\n" +
               $"Last Update: {data["last_update"]}\n" +
               $"Children Killed: {data["killed"]["children"]}";
    }
}

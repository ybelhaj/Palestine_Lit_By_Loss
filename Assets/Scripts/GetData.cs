//Applied to 'DataRetrieval' GameObject

using System;
using System.Collections;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using TMPro;

public class GetData : MonoBehaviour
{
    public string DataURL;

    public TextMeshProUGUI gazaDeathTollText;
    public TextMeshProUGUI gazaLastUpdateText;
    public TextMeshProUGUI westBankDeathTollText;
    public TextMeshProUGUI westBankLastUpdateText;

    public float counterDuration = 1.5f;

    public int gazaDeaths = -1;
    public int westBankDeaths = -1;

    private string gazaUpdateFormatted = "";
    private string westBankUpdateFormatted = "";

    void Start()
    {
        //Start the data-fetching process
        StartCoroutine(getData());
    }

    //Coroutine to fetch JSON data from URL
    IEnumerator getData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(DataURL))
        {
            yield return request.SendWebRequest();

            //Handle connection errors
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network error: " + request.error);
            }
            else
            {
                //If successful, parse the downloaded JSON text
                string json = request.downloadHandler.text;
                ReadJSON(json);
            }
        }
    }

    //Parse JSON and extract relevant data
    void ReadJSON(string jsonString)
    {
        //Parse JSON string and convert it to an object
        JSONNode node = JSON.Parse(jsonString); 
        JSONObject obj = node.AsObject;

        //Try extract the number of children killed in each region
        int.TryParse(obj["gaza"]["killed"]["children"].Value, out gazaDeaths);
        int.TryParse(obj["west_bank"]["killed"]["children"].Value, out westBankDeaths);

        //Extract and try format 'last_update' date strings
        string gazaUpdateRaw = obj["gaza"]["last_update"].Value;
        string westBankUpdateRaw = obj["west_bank"]["last_update"].Value;

        gazaUpdateFormatted = TryFormatDate(gazaUpdateRaw);
        westBankUpdateFormatted = TryFormatDate(westBankUpdateRaw);

        //Update UI with formatted date text
        if (gazaLastUpdateText != null)
            gazaLastUpdateText.text = $"*Dataset updated: {gazaUpdateFormatted}";

        if (westBankLastUpdateText != null)
            westBankLastUpdateText.text = $"*Dataset updated: {westBankUpdateFormatted}";

        Debug.Log($"Data loaded. Gaza child deaths: {gazaDeaths}, West Bank child deaths: {westBankDeaths}");
    }

    //Try convert raw date string into a readable format
    string TryFormatDate(string raw)
    {
        if (DateTime.TryParse(raw, out DateTime parsed))
            return parsed.ToString("dd MMMM yyyy");

        return raw;
    }

    //Public function called from ImageTracker script to start death toll counter animation for each region
    public void AnimateRegionCounters(string regionName)
    {
        if (regionName == "Gaza" && gazaDeathTollText != null && gazaDeaths >= 0)
        {
            StopCoroutine("AnimateCounter");
            StartCoroutine(AnimateCounter(gazaDeathTollText, gazaDeaths));
        }

        if (regionName == "WestBank" && westBankDeathTollText != null && westBankDeaths >= 0)
        {
            StopCoroutine("AnimateCounter");
            StartCoroutine(AnimateCounter(westBankDeathTollText, westBankDeaths));
        }
    }

    //Coroutine that animates number from 0 to the final death toll value
    IEnumerator AnimateCounter(TextMeshProUGUI textField, int finalValue)
    {
        float elapsed = 0f;
        float startValue = 0f;

        while (elapsed < counterDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / counterDuration);
            int current = Mathf.RoundToInt(Mathf.Lerp(startValue, finalValue, t));
            textField.text = current.ToString("N0");
            yield return null;
        }

        //Makes sure the final number is displayed exactly
        textField.text = finalValue.ToString("N0");
    }
}
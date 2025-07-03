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

    private int gazaDeaths = -1;
    private int westBankDeaths = -1;

    private string gazaUpdateFormatted = "";
    private string westBankUpdateFormatted = "";

    void Start()
    {
        StartCoroutine(getData());
    }

    IEnumerator getData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(DataURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("❌ Network error: " + request.error);
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
        JSONObject obj = node.AsObject;

        int.TryParse(obj["gaza"]["killed"]["children"].Value, out gazaDeaths);
        int.TryParse(obj["west_bank"]["killed"]["children"].Value, out westBankDeaths);

        string gazaUpdateRaw = obj["gaza"]["last_update"].Value;
        string westBankUpdateRaw = obj["west_bank"]["last_update"].Value;

        gazaUpdateFormatted = TryFormatDate(gazaUpdateRaw);
        westBankUpdateFormatted = TryFormatDate(westBankUpdateRaw);

        if (gazaLastUpdateText != null)
            gazaLastUpdateText.text = $"*Dataset updated: {gazaUpdateFormatted}";

        if (westBankLastUpdateText != null)
            westBankLastUpdateText.text = $"*Dataset updated: {westBankUpdateFormatted}";

        Debug.Log($"✅ Data loaded. Gaza: {gazaDeaths}, WB: {westBankDeaths}");
    }

    string TryFormatDate(string raw)
    {
        if (DateTime.TryParse(raw, out DateTime parsed))
            return parsed.ToString("dd MMMM yyyy");
        return raw;
    }

    public void AnimateRegionCounters(string region)
    {
        if (region == "Gaza" && gazaDeathTollText != null && gazaDeaths >= 0)
        {
            StopCoroutine("AnimateCounter"); // Optional cleanup
            StartCoroutine(AnimateCounter(gazaDeathTollText, gazaDeaths));
        }

        if (region == "WestBank" && westBankDeathTollText != null && westBankDeaths >= 0)
        {
            StopCoroutine("AnimateCounter"); // Optional cleanup
            StartCoroutine(AnimateCounter(westBankDeathTollText, westBankDeaths));
        }
    }

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

        textField.text = finalValue.ToString("N0");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;

public class GetChild : MonoBehaviour
{
    public string DataURL = "https://data.techforpalestine.org/api/v2/killed-in-gaza.min.json";

    public GameObject namesCanvas;
    public GameObject homeButton;
    public GameObject startNamesButton;
    public GameObject namesPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI infoText;
    public Button nextButton;
    public Button backButton;

    private List<JSONNode> childrenList = new List<JSONNode>();
    private int currentIndex = 0;
    private bool dataReady = false;

    void Start()
    {
        namesCanvas.SetActive(false);
    }

    IEnumerator GetDataFromWeb()
    {
        Debug.Log("📡 Starting web request to: " + DataURL);

        using (UnityWebRequest request = UnityWebRequest.Get(DataURL))
        {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                infoText.text = "Error fetching data.";
                Debug.LogError("❌ Network error: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                Debug.Log("✅ Successfully fetched data. Length: " + json.Length);

                try
                {
                    ProcessJSON(json);
                }
                catch (Exception ex)
                {
                    infoText.text = "Error reading data.";
                    Debug.LogError("❌ JSON parsing failed: " + ex.Message);
                }
            }
        }
    }

    IEnumerator WaitThenGetData()
    {
        yield return new WaitForEndOfFrame();
        yield return GetDataFromWeb();
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("▶️ Start button clicked.");

        startNamesButton.SetActive(false);
        namesPanel.SetActive(true);
        homeButton.SetActive(true);
        infoText.text = "Loading data...";
        nextButton.interactable = false;
        backButton.interactable = false;
        StartCoroutine(WaitThenGetData());
    }

    public void OnHomeButtonClicked()
    {
        namesPanel.SetActive(false);
        startNamesButton.SetActive(true);
        homeButton.SetActive(false);
    }

    void ProcessJSON(string jsonString)
    {
        JSONNode root = JSON.Parse(jsonString);
        if (root == null)
        {
            throw new Exception("Root JSON is null or malformed.");
        }

        JSONArray peopleArray = root.AsArray;
        if (peopleArray == null)
        {
            throw new Exception("Expected a JSON array at root.");
        }

        childrenList.Clear();

        foreach (JSONNode person in peopleArray)
        {
            if (person.HasKey("age") && int.TryParse(person["age"], out int age) && age < 18)
            {
                childrenList.Add(person);
            }
        }

        Debug.Log($"👶 Filtered {childrenList.Count} children under age 18.");

        Shuffle(childrenList);

        currentIndex = 0;
        dataReady = true;
        nextButton.interactable = true;

        ShowNextChild();
    }

    public void ShowNextChild()
    {
        if (!dataReady || currentIndex >= childrenList.Count)
        {
            infoText.text = "You've reached the end of the list.";
            nextButton.interactable = false;
            return;
        }

        JSONNode person = childrenList[currentIndex];
        currentIndex++;

        string name = person["en_name"];
        string age = person["age"];
        string gender = (person["sex"] == "m" ? "Boy" : "Girl");
        string dobRaw = person["dob"];
        string dobFormatted = dobRaw;

        if (DateTime.TryParse(dobRaw, out DateTime parsedDate))
        {
            dobFormatted = parsedDate.ToString("dd MMMM yyyy");
        }

        nameText.text = name;
        infoText.text = $"DOB: {dobFormatted}\n" +
                        $"Gender: {gender}\n\n" +
                        $"Killed at the age of {age}.";

        backButton.interactable = currentIndex > 1;
        nextButton.interactable = currentIndex < childrenList.Count;
    }

    public void ShowPreviousChild()
    {
        if (!dataReady || currentIndex <= 1)
        {
            currentIndex = 0;
            backButton.interactable = false;
            return;
        }

        currentIndex -= 2;
        ShowNextChild();
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}

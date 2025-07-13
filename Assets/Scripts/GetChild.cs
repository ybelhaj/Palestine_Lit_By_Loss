//Applied to 'ChildRetrieval' GameObject

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;

public class GetChild : MonoBehaviour
{
    //URL for the Gaza child name data (JSON)
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
        // Hide names UI at start
        namesCanvas.SetActive(false); 
    }

    //Called when user clicks 'Start' to load and display names
    public void OnStartButtonClicked()
    {
        startNamesButton.SetActive(false);
        namesPanel.SetActive(true);
        homeButton.SetActive(true);
        infoText.text = "Loading data...";
        nextButton.interactable = false;
        backButton.interactable = false;

        StartCoroutine(GetDataFromWeb());
    }

    //Called when user clicks the 'Home' button to return to the prompt screen
    public void OnHomeButtonClicked()
    {
        namesPanel.SetActive(false);
        startNamesButton.SetActive(true);
        homeButton.SetActive(false);
    }

    //Coroutine to fetch JSON data from URL
    IEnumerator GetDataFromWeb()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(DataURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network error: " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                ReadJSON(json);
            }
        }
    }

    //Parse JSON and filters out children under 18
    void ReadJSON(string jsonString)
    {
        JSONArray peopleArray = JSON.Parse(jsonString).AsArray;
        childrenList.Clear();

        foreach (JSONNode person in peopleArray)
        {
            if (person.HasKey("age") && int.TryParse(person["age"], out int age) && age < 18)
            {
                childrenList.Add(person);
            }
        }

        Debug.Log($"Filtered children under age 18: {childrenList.Count}");

        Shuffle(childrenList);
        currentIndex = 0;
        dataReady = true;
        nextButton.interactable = true;

        ShowNextChild();
    }

    //Display next child's info in UI
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

    //Go back to previous child
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

    //Randomise list using Fisher-Yates shuffle
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
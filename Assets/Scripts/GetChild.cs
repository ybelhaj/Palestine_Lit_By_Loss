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

    public GameObject homeButton;           //Button return to intro
    public GameObject startNamesButton;    //'Know Their Names' button
    public GameObject namesPanel;          //Panel that holds infoText, nameText and buttons 
    public TextMeshProUGUI nameText;       //TextMeshProUGUI for displaying name
    public TextMeshProUGUI infoText;       //TextMeshProUGUI for displaying info
    public Button nextButton;              //UI Button to go to next child
    public Button backButton;              //UI Button to go back


    private List<JSONNode> childrenList = new List<JSONNode>();
    private int currentIndex = 0;
    private bool dataReady = false;

    void Start()
    {
        namesPanel.SetActive(false);        //Hide NamesPanel UI at start
        startNamesButton.SetActive(true);   //Show the 'Know their names' button
        homeButton.SetActive(false); // Hide Home button at start
    }

    IEnumerator GetDataFromWeb()
    {
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
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                ProcessJSON(json);
            }
        }
    }

    public void OnStartButtonClicked()
    {
        startNamesButton.SetActive(false);       // Hide the start button
        namesPanel.SetActive(true);              // Show the main UI panel
        homeButton.SetActive(true);              // Show Home button
        infoText.text = "Loading data...";       // Optional loading message
        nextButton.interactable = false;
        backButton.interactable = false;
        StartCoroutine(GetDataFromWeb());        // Start fetching
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
        JSONArray peopleArray = root.AsArray;

        childrenList.Clear();

        foreach (JSONNode person in peopleArray)
        {
            if (person.HasKey("age") && int.TryParse(person["age"], out int age) && age < 18)
            {
                childrenList.Add(person);
            }
        }

        Shuffle(childrenList);
        ExportToFile(childrenList, "ShuffledChildrenList.txt");

        currentIndex = 0;
        dataReady = true;
        nextButton.interactable = true;

        ShowNextChild(); // Show the first person
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
            dobFormatted = parsedDate.ToString("dd MMMM yyyy"); // → e.g., 10 May 2021
        }

        nameText.text = name; 
        infoText.text = $"DOB: {dobFormatted}\n" +
                        $"Gender: {gender}\n\n" +
                        $"Killed at the age of {age}.";

        //Button state updates
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

        currentIndex -= 2;      //Step back two because ShowNextChild will increment
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

    void ExportToFile(List<JSONNode> list, string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, fileName);
        List<string> lines = new List<string>();

        lines.Add("Name,Age,Gender,DateOfBirth");

        foreach (var person in list)
        {
            string name = EscapeCsv(person["en_name"]);
            string age = person["age"];
            string gender = (person["sex"] == "m" ? "Boy" : "Girl");
            string dob = person["dob"];

            string line = $"{name},{age},{gender},{dob}";
            lines.Add(line);
        }

        File.WriteAllLines(filePath, lines.ToArray());
        Debug.Log($"📄 Exported {list.Count} entries to {filePath}");
    }

    string EscapeCsv(string value)
    {
        if (value.Contains(",") || value.Contains("\""))
        {
            value = value.Replace("\"", "\"\"");
            value = $"\"{value}\"";
        }
        return value;
    }
}

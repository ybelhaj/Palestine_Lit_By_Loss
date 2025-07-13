//Applied to 'XR Origin' GameObject

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class TrackImage : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;

    public GameObject scanPrompt;
    public GameObject regionPrompt;
    public GameObject mapPrefab;
    public GameObject namesCanvas;

    public GameObject gazaCandlePrompt;
    public GameObject westBankCandlePrompt;

    public GameObject gazaRegionInfo;
    public GameObject westBankRegionInfo;

    public GetData getData;
    public CandleSpawner candleSpawner;

    public Button backToMapButton;
    public RegionInfoButtonManager regionInfoButtonManager;

    public static Vector3 LastSpawnPosition { get; private set; }

    private GameObject spawnedMap;
    public string CurrentRegion { get; private set; } = "";

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    //Called when AR Foundation updates tracked images
    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage newImage in eventArgs.added)
        {
            //Instantiate map at image position
            spawnedMap = Instantiate(mapPrefab);
            Vector3 spawnPosition = newImage.transform.position;
            Quaternion spawnRotation = newImage.transform.rotation;

            spawnedMap.transform.position = spawnPosition;

            //Rotate map to face the user (only horizontally)
            Transform cameraTransform = Camera.main?.transform;
            if (cameraTransform != null)
            {
                Vector3 lookDirection = spawnPosition - cameraTransform.position;
                lookDirection.y = 0f; //Lock Y so it doesn’t tilt
                if (lookDirection.sqrMagnitude > 0.001f)
                    spawnedMap.transform.rotation = Quaternion.LookRotation(lookDirection.normalized);
            }
            else
            {
                //Use tracked image rotation as back-up
                spawnedMap.transform.rotation = spawnRotation;
            }

            //Update UI states after tracking is successful
            if (scanPrompt != null) scanPrompt.SetActive(false);
            if (regionPrompt != null) regionPrompt.SetActive(true);

            if (namesCanvas != null)
            {
                namesCanvas.SetActive(true);
                GameObject startButton = namesCanvas.transform.Find("StartNamesButton")?.gameObject;
                if (startButton != null) startButton.SetActive(false);
            }

            //Hide all region prompts and info panels
            if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
            if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
            if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
            if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

            if (backToMapButton != null)
                backToMapButton.gameObject.SetActive(false);

            //Save spawn position for candle placement
            LastSpawnPosition = spawnPosition;
        }
    }

    //Called when a region is selected
    public void ShowCandlePrompt(string regionName)
    {
        CurrentRegion = regionName;

        //Hide all region prompts and info panels
        if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
        if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
        if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
        if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

        //Show the relevant prompts and info for selected region
        if (regionName == "Gaza")
        {
            if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(true);
            if (gazaRegionInfo != null) gazaRegionInfo.SetActive(true);
        }
        else if (regionName == "WestBank")
        {
            if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(true);
            if (westBankRegionInfo != null) westBankRegionInfo.SetActive(true);
        }

        //Trigger number counter animation for child deaths in selected region
        if (getData != null)
            getData.AnimateRegionCounters(regionName);

        //Spawn candles based on death toll
        if (candleSpawner != null && getData != null)
        {
            int deathCount = regionName == "Gaza" ? getData.gazaDeaths : getData.westBankDeaths;
            Vector3 candleCenter = new Vector3(LastSpawnPosition.x, spawnedMap.transform.position.y, LastSpawnPosition.z);
            candleSpawner.SpawnCandles(deathCount, candleCenter);
        }
    }

    //Called when the "Return to Main Map" button is pressed
    public void ReturnToMainMap()
    {
        CurrentRegion = "";

        if (regionPrompt != null) regionPrompt.SetActive(true);

        if (namesCanvas != null)
        {
            namesCanvas.SetActive(true);
            GameObject startButton = namesCanvas.transform.Find("StartNamesButton")?.gameObject;
            if (startButton != null) startButton.SetActive(false);
        }

        if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
        if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
        if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
        if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

        //Clear all spawned candles
        if (candleSpawner != null)
            candleSpawner.ClearAllCandles();

        if (backToMapButton != null)
            backToMapButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (namesCanvas != null)
        {
            GameObject startButton = namesCanvas.transform.Find("StartNamesButton")?.gameObject;
            GameObject namesPanel = namesCanvas.transform.Find("NamesPanel")?.gameObject;

            bool isGaza = CurrentRegion == "Gaza";
            bool isGazaInfoClosed = gazaRegionInfo != null && !gazaRegionInfo.activeSelf;
            bool isNamesPanelClosed = namesPanel == null || !namesPanel.activeSelf;

            //Only show 'Know their names' button for Gaza if info panel and name panel closed
            if (startButton != null)
                startButton.SetActive(isGaza && isGazaInfoClosed && isNamesPanelClosed);
        }
    }
}

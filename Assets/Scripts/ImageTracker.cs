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
    private string currentRegion = ""; // 🆕 Track selected region

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;
    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage newImage in eventArgs.added)
        {
            spawnedMap = Instantiate(mapPrefab);

            Vector3 spawnPosition = newImage.transform.position;
            Quaternion spawnRotation = newImage.transform.rotation;
            spawnedMap.transform.position = spawnPosition;

            Transform cameraTransform = Camera.main != null ? Camera.main.transform : null;
            if (cameraTransform != null)
            {
                Vector3 lookDirection = cameraTransform.position - spawnPosition;
                lookDirection.y = 0f;

                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(lookDirection.normalized);
                    spawnedMap.transform.rotation = lookRotation;
                }
            }
            else
            {
                spawnedMap.transform.rotation = spawnRotation;
            }

            if (scanPrompt != null) scanPrompt.SetActive(false);
            if (regionPrompt != null) regionPrompt.SetActive(true);

            if (namesCanvas != null)
            {
                namesCanvas.SetActive(true);

                // 🔻 Hide StartNamesButton initially
                GameObject startButton = namesCanvas.transform.Find("StartNamesButton")?.gameObject;
                if (startButton != null)
                    startButton.SetActive(false);
            }

            if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
            if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
            if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
            if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

            if (backToMapButton != null)
                backToMapButton.gameObject.SetActive(false);

            LastSpawnPosition = spawnPosition;
        }
    }

    public void ShowCandlePrompt(string regionName)
    {
        if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
        if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
        if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
        if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

        currentRegion = regionName; // 🆕 track current region

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

        if (regionInfoButtonManager != null)
            regionInfoButtonManager.SetCurrentRegion(regionName);

        if (getData != null)
            getData.AnimateRegionCounters(regionName);

        if (candleSpawner != null && getData != null)
        {
            int deathCount = 0;
            if (regionName == "Gaza")
                deathCount = getData.gazaDeaths;
            else if (regionName == "WestBank")
                deathCount = getData.westBankDeaths;

            Vector3 candleCenter = new Vector3(LastSpawnPosition.x, spawnedMap.transform.position.y, LastSpawnPosition.z);
            candleSpawner.SpawnCandles(deathCount, candleCenter);
        }
    }

    public void ReturnToMainMap()
    {
        if (regionPrompt != null) regionPrompt.SetActive(true);

        currentRegion = ""; // 🆕 reset region

        if (namesCanvas != null)
        {
            namesCanvas.SetActive(true);

            // ❌ Hide StartNamesButton on return
            GameObject startButton = namesCanvas.transform.Find("StartNamesButton")?.gameObject;
            if (startButton != null)
                startButton.SetActive(false);
        }

        if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
        if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
        if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
        if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

        if (candleSpawner != null)
            candleSpawner.ClearAllCandles();

        if (regionInfoButtonManager != null)
            regionInfoButtonManager.ClearRegion();

        RegionSelector[] regionSelectors = FindObjectsOfType<RegionSelector>();
        foreach (var selector in regionSelectors)
        {
            selector.ResetRegion();
        }

        if (backToMapButton != null)
            backToMapButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (namesCanvas != null)
        {
            GameObject startButton = namesCanvas.transform.Find("StartNamesButton")?.gameObject;
            GameObject namesPanel = namesCanvas.transform.Find("NamesPanel")?.gameObject;

            if (startButton != null)
            {
                bool isGaza = currentRegion == "Gaza";
                bool isGazaInfoClosed = gazaRegionInfo != null && !gazaRegionInfo.activeSelf;
                bool isNamesPanelClosed = namesPanel == null || !namesPanel.activeSelf;

                bool shouldShow = isGaza && isGazaInfoClosed && isNamesPanelClosed;
                startButton.SetActive(shouldShow);
            }
        }
    }
}

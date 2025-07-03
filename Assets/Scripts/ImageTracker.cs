using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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

    public ARSession arSession;

    public GetData getData; // ✅ Assign in Inspector

    public CandleSpawner candleSpawner; // ✅ Assign in Inspector

    public static Vector3 LastSpawnPosition { get; private set; }

    private GameObject spawnedMap;

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
            if (namesCanvas != null) namesCanvas.SetActive(true);

            // ✅ Hide all prompts and info panels initially
            if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
            if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
            if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
            if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

            LastSpawnPosition = spawnPosition;
        }
    }

    public void ShowCandlePrompt(string regionName)
    {
        // ✅ Hide all prompts and panels first
        if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
        if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
        if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
        if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

        // ✅ Show correct region's prompts and info
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

        // ✅ Animate counters via GetData
        if (getData != null)
            getData.AnimateRegionCounters(regionName);

        // ✅ Spawn candles for the selected region
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

    public void ResetMap()
    {
        if (spawnedMap != null)
        {
            Destroy(spawnedMap);
            spawnedMap = null;
        }

        if (scanPrompt != null) scanPrompt.SetActive(true);
        if (regionPrompt != null) regionPrompt.SetActive(false);
        if (namesCanvas != null) namesCanvas.SetActive(false);

        // ✅ Hide all prompts and panels
        if (gazaCandlePrompt != null) gazaCandlePrompt.SetActive(false);
        if (westBankCandlePrompt != null) westBankCandlePrompt.SetActive(false);
        if (gazaRegionInfo != null) gazaRegionInfo.SetActive(false);
        if (westBankRegionInfo != null) westBankRegionInfo.SetActive(false);

        // ✅ Clear candles
        if (candleSpawner != null)
            candleSpawner.ClearAllCandles();

        StartCoroutine(ResetARSession());
    }

    private IEnumerator ResetARSession()
    {
        if (arSession != null)
        {
            arSession.Reset();
            yield return null;
        }
        else
        {
            Debug.LogWarning("⚠️ ARSession reference not assigned.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackImage : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;

    public GameObject scanPrompt;    // Reference to Prompt A
    public GameObject regionPrompt;  // Reference to Prompt B
    public GameObject mapPrefab;     // Prefab to appear on marker image
    public GameObject namesCanvas;   // Reference to the full NamesCanvas UI
    public ARSession arSession;      // Reference to the AR Session for reset

    public static Vector3 LastSpawnPosition { get; private set; }

    private GameObject spawnedMap; // Store the spawned map instance

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage newImage in eventArgs.added)
        {
            // Instantiate and track the map prefab
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

            // Update UI
            if (scanPrompt != null) scanPrompt.SetActive(false);
            if (regionPrompt != null) regionPrompt.SetActive(true);
            if (namesCanvas != null) namesCanvas.SetActive(true);

            LastSpawnPosition = spawnPosition;
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

        StartCoroutine(ResetARSession());
    }

    private IEnumerator ResetARSession()
    {
        if (arSession != null)
        {
            arSession.Reset();
            yield return null; // Wait one frame (optional)
        }
        else
        {
            Debug.LogWarning("⚠️ ARSession reference not assigned.");
        }
    }
}

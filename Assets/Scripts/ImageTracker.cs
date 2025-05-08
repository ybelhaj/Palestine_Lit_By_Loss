using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // include XR library

public class TrackImage : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;

    public GameObject scanPrompt;    // Reference to Prompt A
    public GameObject regionPrompt;  // Reference to Prompt B
    public GameObject mapPrefab;     // Prefab to appear on marker image
    public GameObject namesCanvas;   // NEW: Reference to the full NamesCanvas UI

    public static Vector3 LastSpawnPosition { get; private set; } // Public static spawn position

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage newImage in eventArgs.added)
        {
            // Instantiate the map prefab
            GameObject newObject = Instantiate(mapPrefab);

            // Snapshot the tracked image’s position and rotation
            Vector3 spawnPosition = newImage.transform.position;
            Quaternion spawnRotation = newImage.transform.rotation;

            // Set the initial position
            newObject.transform.position = spawnPosition;

            // Face the camera horizontally (Y-axis only)
            Transform cameraTransform = Camera.main != null ? Camera.main.transform : null;

            if (cameraTransform != null)
            {
                Vector3 lookDirection = cameraTransform.position - spawnPosition;
                lookDirection.y = 0f; // Keep it horizontal

                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(lookDirection.normalized);
                    newObject.transform.rotation = lookRotation;
                }
            }
            else
            {
                newObject.transform.rotation = spawnRotation;
            }

            // Hide the scan prompt and show the region prompt
            if (scanPrompt != null)
                scanPrompt.SetActive(false);

            if (regionPrompt != null)
                regionPrompt.SetActive(true);

            // NEW: Show the full Names UI
            if (namesCanvas != null)
                namesCanvas.SetActive(true);

            // Store spawn position globally
            LastSpawnPosition = spawnPosition;
        }
    }
}

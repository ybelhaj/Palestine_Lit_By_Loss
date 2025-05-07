using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // include XR library

public class TrackImage : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;

    public GameObject mapPrefab; // Prefab to appear on marker image

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

                    // Apply rotation
                    newObject.transform.rotation = lookRotation;

                    // Debug logs
                    //Debug.Log("Look direction: " + lookDirection);
                    //Debug.Log("Applied rotation: " + lookRotation.eulerAngles);
                }
            }
            else
            {
                // Fallback
                newObject.transform.rotation = spawnRotation;
                //Debug.LogWarning("Camera.main not found — using tracked image rotation instead.");
}

            // Store spawn position globally for use by RegionSelector
            LastSpawnPosition = spawnPosition;
        }
    }
}

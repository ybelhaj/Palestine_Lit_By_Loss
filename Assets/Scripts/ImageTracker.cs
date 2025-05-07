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

            // Set the transform once
            newObject.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

            // Store spawn position globally for use by RegionSelector
            LastSpawnPosition = spawnPosition;
        }
    }
}

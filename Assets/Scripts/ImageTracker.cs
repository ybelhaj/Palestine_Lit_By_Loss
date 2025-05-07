using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // include xr library

public class TrackImage : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager; 
    public GameObject mapPrefab; //Prefab you want to appear on marker image
    
    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage newImage in eventArgs.added)
        {
            // Instantiate the map prefab
            GameObject newObject = Instantiate(mapPrefab);

            // Snapshot the tracked image’s current position and rotation
            Vector3 spawnPosition = newImage.transform.position;
            Quaternion spawnRotation = newImage.transform.rotation;

            // Set the map's transform once
            newObject.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

            // DO NOT parent the map to the marker
            // DO NOT update it in any Update() or OnChanged loop afterward
        }
    }

}
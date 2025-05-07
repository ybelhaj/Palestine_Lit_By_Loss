using UnityEngine;

public class RegionSelector : MonoBehaviour
{
    public GameObject[] otherRegions;    // Other regions to hide
    public Vector3 targetScale = Vector3.one;  // Optional: not used unless you want scaling
    public Vector3 targetPosition = Vector3.zero;
    public Quaternion targetRotation = Quaternion.Euler(0, 0, 0);  // Flat rotation

    public float moveSpeed = 2.0f;
    private bool isSelected = false;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float lerpProgress = 0f;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void OnMouseDown()
    {
        if (!isSelected)
        {
            // Hide other regions
            foreach (GameObject region in otherRegions)
            {
                region.SetActive(false);
            }

            // Set the target position to the map's spawn point
            targetPosition = TrackImage.LastSpawnPosition;

            isSelected = true;
        }
    }

    void Update()
    {
        if (isSelected && lerpProgress < 1f)
        {
            lerpProgress += Time.deltaTime * moveSpeed;
            lerpProgress = Mathf.Clamp01(lerpProgress);

            transform.position = Vector3.Lerp(initialPosition, targetPosition, lerpProgress);
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, lerpProgress);
            // Optional: transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpProgress);
        }
    }
}

using UnityEngine;

public class RegionSelector : MonoBehaviour
{
    public GameObject[] otherRegions;    // Other regions to hide
    public Vector3 targetScale = Vector3.one;  // New scale for the selected region
    public Vector3 targetPosition = Vector3.zero;  // New position for the selected region

    public float moveSpeed = 2.0f;   // How fast to move/scale
    private bool isSelected = false;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private float lerpProgress = 0f;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
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

            // Start the smooth resizing and moving
            isSelected = true;
        }
    }

    void Update()
    {
        if (isSelected && lerpProgress < 1f)
        {
            lerpProgress += Time.deltaTime * moveSpeed;
            lerpProgress = Mathf.Clamp01(lerpProgress);

            transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpProgress);
            transform.position = Vector3.Lerp(initialPosition, targetPosition, lerpProgress);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class RegionSelector : MonoBehaviour
{
    public GameObject[] otherRegions;
    public Vector3 targetScale = Vector3.one;
    public Vector3 targetPosition = Vector3.zero;
    public Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

    public float moveSpeed = 2.0f;

    private bool isSelected = false;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float lerpProgress = 0f;

    private GameObject regionPrompt;
    private Button backMapButton;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        regionPrompt = GameObject.Find("RegionPrompt");

        GameObject buttonObj = GameObject.FindWithTag("MapBackButton");

        if (buttonObj != null)
        {
            backMapButton = buttonObj.GetComponent<Button>();
            backMapButton.gameObject.SetActive(false);
            backMapButton.onClick.AddListener(ReturnToMap);
        }
    }

    void OnMouseDown()
    {
        if (!isSelected)
        {
            foreach (GameObject region in otherRegions)
            {
                if (region != null)
                    region.SetActive(false);
            }

            if (regionPrompt != null)
                regionPrompt.SetActive(false);

            if (backMapButton != null)
                backMapButton.gameObject.SetActive(true);

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
            transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpProgress);
        }
    }

    void ReturnToMap()
    {
        // Reactivate all regions (this + others)
        foreach (GameObject region in otherRegions)
        {
            if (region != null)
                region.SetActive(true);
        }

        gameObject.SetActive(true); // This region too, in case it was hidden

        // Reset this region’s transform
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        // Reset state
        isSelected = false;
        lerpProgress = 0f;

        // Hide the back button
        if (backMapButton != null)
            backMapButton.gameObject.SetActive(false);

        // Optionally re-show the region prompt
        if (regionPrompt != null)
            regionPrompt.SetActive(true);
    }
}

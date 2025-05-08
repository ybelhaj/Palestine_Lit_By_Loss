using UnityEngine;
using TMPro;

public class RegionSelector : MonoBehaviour
{
    public string regionKey; // "gaza" or "west_bank"
    public GameObject[] otherRegions;
    public Vector3 targetScale = Vector3.one;
    public float moveSpeed = 2.0f;

    private bool isSelected = false;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float lerpProgress = 0f;
    private GameObject regionPrompt;
    private TextMeshProUGUI infoTextDisplay;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Optional: find region prompt in scene
        regionPrompt = GameObject.Find("RegionPrompt");

        // Find the UI text element dynamically
        GameObject infoTextObject = GameObject.Find("InfoTextDisplay");
        if (infoTextObject != null)
        {
            infoTextDisplay = infoTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogWarning("⚠ InfoTextDisplay UI element not found in scene.");
        }
    }

    void OnMouseDown()
    {
        if (isSelected) return;

        foreach (GameObject region in otherRegions)
        {
            if (region != null) region.SetActive(false);
        }

        if (regionPrompt != null)
            regionPrompt.SetActive(false);

        targetPosition = TrackImage.LastSpawnPosition;

        Transform cam = Camera.main?.transform;
        if (cam != null)
        {
            Vector3 lookDir = cam.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
                targetRotation = Quaternion.LookRotation(lookDir);
        }

        if (infoTextDisplay != null)
        {
            infoTextDisplay.text = GetDataHelper.GetFormattedInfo(regionKey);
        }

        isSelected = true;
    }

    void Update()
    {
        if (!isSelected || lerpProgress >= 1f) return;

        lerpProgress += Time.deltaTime * moveSpeed;
        lerpProgress = Mathf.Clamp01(lerpProgress);

        transform.position = Vector3.Lerp(initialPosition, targetPosition, lerpProgress);
        transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpProgress);
    }
}

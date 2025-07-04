using UnityEngine;
using UnityEngine.UI;

public class RegionSelector : MonoBehaviour
{
    public GameObject[] otherRegions;

    public Vector3 targetScale = Vector3.one * 1.5f;
    public float floatHeight = 0.5f;
    public float floatAmplitude = 0.05f;
    public float floatSpeed = 2f;
    public float moveSpeed = 2f;

    public string regionName; // ✅ NEW: Set in Inspector: "Gaza" or "WestBank"

    private bool isSelected = false;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float lerpProgress = 0f;
    private float floatStartY;
    private float floatTimeOffset;

    private GameObject regionPrompt;
    private Button backMapButton;
    private TrackImage trackImage;

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

        floatTimeOffset = Random.Range(0f, 2f * Mathf.PI);
        trackImage = Object.FindAnyObjectByType<TrackImage>();
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

            isSelected = true;
            lerpProgress = 0f;
            floatStartY = TrackImage.LastSpawnPosition.y + floatHeight;

            // ✅ Tell TrackImage to show the right prompt
            if (trackImage != null)
                trackImage.ShowCandlePrompt(regionName);
        }
    }

    void Update()
    {
        if (isSelected && lerpProgress < 1f)
        {
            lerpProgress += Time.deltaTime * moveSpeed;
            lerpProgress = Mathf.Clamp01(lerpProgress);

            Vector3 targetPosition = TrackImage.LastSpawnPosition + new Vector3(0, floatHeight, 0);
            transform.position = Vector3.Lerp(initialPosition, targetPosition, lerpProgress);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpProgress);
        }

        if (isSelected && lerpProgress >= 1f)
        {
            Vector3 pos = transform.position;
            pos.y = floatStartY + Mathf.Sin(Time.time * floatSpeed + floatTimeOffset) * floatAmplitude;
            transform.position = pos;
        }
    }

    void ReturnToMap()
    {
        foreach (GameObject region in otherRegions)
        {
            if (region != null)
                region.SetActive(true);
        }

        gameObject.SetActive(true);
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        isSelected = false;
        lerpProgress = 0f;

        if (backMapButton != null)
            backMapButton.gameObject.SetActive(false);

        if (regionPrompt != null)
            regionPrompt.SetActive(true);
    }
}

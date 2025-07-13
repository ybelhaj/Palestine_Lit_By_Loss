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

    public string regionName;

    private GameObject regionPrompt;
    private TrackImage trackImage;

    private bool isSelected = false;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float lerpProgress = 0f;
    private float floatStartY;
    private float floatTimeOffset;

    void Start()
    {
        //Save the region's starting position, scale, and rotation for resetting later
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        //Reference for the regionPrompt for hiding it later
        regionPrompt = GameObject.Find("RegionPrompt");

        //Find the TrackImage script in the scene
        trackImage = Object.FindAnyObjectByType<TrackImage>();

        //If TrackImage and its backToMapButton exist, then hide the button
        //And add an event listener for when the button is clicked
        if (trackImage != null && trackImage.backToMapButton != null)
        {
            trackImage.backToMapButton.gameObject.SetActive(false);
            trackImage.backToMapButton.onClick.AddListener(ReturnToMap);
        }
    }

    void OnMouseDown()
    {
        if (!isSelected)
        {
            //Hide unselected region
            foreach (GameObject region in otherRegions)
            {
                if (region != null)
                    region.SetActive(false);
            }

            //Hide regionPrompt
            if (regionPrompt != null)
                regionPrompt.SetActive(false);

            //Display the backToMapButton
            if (trackImage != null && trackImage.backToMapButton != null)
                trackImage.backToMapButton.gameObject.SetActive(true);

            //Mark region as selected for Lerp animation
            isSelected = true;
            lerpProgress = 0f;

            //Calculate Y position where the region will start floating
            floatStartY = TrackImage.LastSpawnPosition.y + floatHeight;

            //Infom TrackImage which region was selected so the relevant CandlePrompt can be displayed
            if (trackImage != null)
                trackImage.ShowCandlePrompt(regionName);
        }
    }

    void Update()
    {
        //Animate the region into position and scale when selected
        if (isSelected && lerpProgress < 1f)
        {
            lerpProgress += Time.deltaTime * moveSpeed;
            lerpProgress = Mathf.Clamp01(lerpProgress);

            Vector3 targetPosition = TrackImage.LastSpawnPosition + new Vector3(0, floatHeight, 0);

            //Animate movement to target position
            transform.position = Vector3.Lerp(initialPosition, targetPosition, lerpProgress);

            //Animate scaling of region
            transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpProgress);
        }

        //If movement animation complete, then begin floating effect using sine wave
        if (isSelected && lerpProgress >= 1f)
        {
            Vector3 pos = transform.position;
            pos.y = floatStartY + Mathf.Sin(Time.time * floatSpeed + floatTimeOffset) * floatAmplitude;
            transform.position = pos;
        }
    }

    //Called when backToMapButton clicked
    void ReturnToMap()
    {
        //Re-activate all other regions
        foreach (GameObject region in otherRegions)
        {
            if (region != null)
                region.SetActive(true);
        }

        //Reset this region's transform
        gameObject.SetActive(true);
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        //Reset flags and Lerp
        isSelected = false;
        lerpProgress = 0f;

        //Hide backToMapButton again
        if (trackImage != null && trackImage.backToMapButton != null)
            trackImage.backToMapButton.gameObject.SetActive(false);

        //Re-show the regionPrompt
        if (regionPrompt != null)
            regionPrompt.SetActive(true);
    }

    //Called from TrackImage script to reset this region manually
    //public void ResetRegion()
    //{
        //transform.position = initialPosition;
        //transform.rotation = initialRotation;
        //transform.localScale = initialScale;
        //gameObject.SetActive(true);

        //isSelected = false;
        //lerpProgress = 0f;

        //if (regionPrompt != null)
        //    regionPrompt.SetActive(true);

        //if (trackImage != null && trackImage.backToMapButton != null)
            //trackImage.backToMapButton.gameObject.SetActive(false);

        //foreach (GameObject region in otherRegions)
        //{
           // if (region != null)
            //    region.SetActive(true);
       // }
    //}
}

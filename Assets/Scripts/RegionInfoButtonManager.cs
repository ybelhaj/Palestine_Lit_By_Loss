//Applied to 'RegionUIManager' GameObject

using UnityEngine;

public class RegionInfoButtonManager : MonoBehaviour
{
    public GameObject gazaInfoPanel;
    public GameObject westBankInfoPanel;

    public GameObject gazaInfoButton;
    public GameObject westBankInfoButton;

    public TrackImage trackImage;

    //Get CurrentRegion from the TrackImage class in the ImageTracker script and assign it to 'regionName'
    //If a region is selected, show its corresponding InfoButton only when its InfoPanel is hidden
    //And hide the InfoButton for the unselected region
    //If no region is selected, hide both InfoButtons
    void Update()
    {
        if (trackImage == null) return;

        string regionName = trackImage.CurrentRegion;

        if (regionName == "Gaza")
        {
            gazaInfoButton.SetActive(!gazaInfoPanel.activeSelf);
            westBankInfoButton.SetActive(false);
        }
        else if (regionName == "WestBank")
        {
            westBankInfoButton.SetActive(!westBankInfoPanel.activeSelf);
            gazaInfoButton.SetActive(false);
        }
        else
        {
            gazaInfoButton.SetActive(false);
            westBankInfoButton.SetActive(false);
        }
    }
}

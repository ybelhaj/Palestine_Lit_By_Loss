//Applied to RegionInfo panel 'Close' buttons

using UnityEngine;

public class RegionInfoCloser : MonoBehaviour
{
    public GameObject regionInfoPanelToHide;

    //Hides regionInfoPanel
    public void CloseInfo()
    {
        if (regionInfoPanelToHide != null)
            regionInfoPanelToHide.SetActive(false);
    }
}

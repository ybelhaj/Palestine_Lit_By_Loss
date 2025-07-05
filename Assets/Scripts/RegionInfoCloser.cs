using UnityEngine;

public class RegionInfoCloser : MonoBehaviour
{
    public GameObject regionInfoPanelToHide;

    public void CloseInfo()
    {
        if (regionInfoPanelToHide != null)
            regionInfoPanelToHide.SetActive(false);

    }
}

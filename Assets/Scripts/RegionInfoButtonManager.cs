using UnityEngine;

public class RegionInfoButtonManager : MonoBehaviour
{
    public GameObject gazaInfoPanel;
    public GameObject westBankInfoPanel;

    public GameObject gazaInfoButton;
    public GameObject westBankInfoButton;

    private string currentRegion = "";

    void Start()
    {
        // Hide both buttons by default
        gazaInfoButton.SetActive(false);
        westBankInfoButton.SetActive(false);
    }

    void Update()
    {
        if (currentRegion == "Gaza")
        {
            gazaInfoButton.SetActive(!gazaInfoPanel.activeSelf);
            westBankInfoButton.SetActive(false);
        }
        else if (currentRegion == "WestBank")
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

    public void SetCurrentRegion(string region)
    {
        currentRegion = region;
    }

    public void ClearRegion()
    {
        currentRegion = "";
    }
}

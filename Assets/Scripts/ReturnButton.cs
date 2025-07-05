using UnityEngine;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    public TrackImage trackImage;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null && trackImage != null)
        {
            btn.onClick.AddListener(OnReturnClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ ReturnButton: Missing Button or TrackImage reference.");
        }
    }

    void OnReturnClicked()
    {
        trackImage.ReturnToMainMap();
    }
}

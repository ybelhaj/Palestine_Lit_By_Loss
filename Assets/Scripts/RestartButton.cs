using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    public TrackImage trackImage;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null && trackImage != null)
        {
            btn.onClick.AddListener(OnRestartClicked);
        }
        else
        {
            Debug.LogWarning("⚠️ RestartButton: Button or TrackImage reference is missing.");
        }
    }

    void OnRestartClicked()
    {
        trackImage.ResetMap();
    }
}

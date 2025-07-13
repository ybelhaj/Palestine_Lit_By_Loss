//Applied to 'Return To Main Map' button

using UnityEngine;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    public TrackImage trackImage; 
     
    void Start()
    {
        //Add event listener for when button is clicked
        Button btn = GetComponent<Button>();
        if (btn != null && trackImage != null)
        {
            btn.onClick.AddListener(OnReturnClicked);
        }
    }

    //When button is clicked, call ReturnToMainMap() function from trackImage script
    void OnReturnClicked()
    {
        trackImage.ReturnToMainMap();
    }
}

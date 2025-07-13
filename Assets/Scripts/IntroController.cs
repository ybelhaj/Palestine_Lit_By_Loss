//Applied to 'IntroCanvas' GameObject

using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour
{
    public CanvasGroup introCanvas;

    public CanvasGroup commemorationPanel;

    public CanvasGroup scanPrompt;

    public GameObject promptBackground;

    public float delayBeforeFade = 5f;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(FadeAfterDelay());
    }

    //Coroutine that waits, fades out the intro, then fades in commemoration panel
    private IEnumerator FadeAfterDelay()
    {
        //Wait before fading
        yield return new WaitForSeconds(delayBeforeFade);

        //Fade out intro panel
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            introCanvas.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        //Fully disable intro interaction
        introCanvas.alpha = 0f;
        introCanvas.interactable = false;
        introCanvas.blocksRaycasts = false;

        //Fade in Commemoration panel next
        if (commemorationPanel != null)
        {
            commemorationPanel.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInCanvasGroup(commemorationPanel, fadeDuration));
        }
    }
    
    //Called from the 'Begin AR Experience' button
    public void OnCommemorationContinue()
    {
        if (commemorationPanel != null)
            StartCoroutine(FadeOutCommemorationThenShowScanPrompt());
    }

    //Fade out commemoration panel, then fade in scanPrompt
    private IEnumerator FadeOutCommemorationThenShowScanPrompt()
    {
        float elapsed = 0f;

        //Fade out commemoration panel
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            commemorationPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        commemorationPanel.alpha = 0f;
        commemorationPanel.interactable = false;
        commemorationPanel.blocksRaycasts = false;
        commemorationPanel.gameObject.SetActive(false);

        //Fade in scan prompt and prompt background panel
        if (scanPrompt != null)
        {
            if (promptBackground != null)
                promptBackground.SetActive(true);

            scanPrompt.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInCanvasGroup(scanPrompt, fadeDuration));
        }
    }

    //Reusable function to fade in any CanvasGroup
    private IEnumerator FadeInCanvasGroup(CanvasGroup canvas, float duration)
    {
        float elapsed = 0f;
        canvas.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        canvas.alpha = 1f;
    }
}
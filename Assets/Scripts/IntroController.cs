using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour
{
    public CanvasGroup introCanvas;
    public float delayBeforeFade = 3f;   // Delay in seconds
    public float fadeDuration = 1f;      // Duration of the fade

    void Start()
    {
        StartCoroutine(FadeAfterDelay());
    }

    private IEnumerator FadeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;
        float startAlpha = introCanvas.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            introCanvas.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        introCanvas.alpha = 0f;
        introCanvas.interactable = false;
        introCanvas.blocksRaycasts = false;
    }
}

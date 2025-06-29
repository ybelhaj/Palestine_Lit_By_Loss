using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour
{
    public CanvasGroup introCanvas;
    public float delayBeforeFade = 3f;
    public float fadeDuration = 1f;

    public CanvasGroup scanPrompt;  // 👈 Now using CanvasGroup for fading

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

        if (scanPrompt != null)
        {
            scanPrompt.gameObject.SetActive(true); // Ensure it's active
            StartCoroutine(FadeInCanvasGroup(scanPrompt, fadeDuration));
        }
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup cg, float duration)
    {
        float elapsed = 0f;
        cg.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            cg.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        cg.alpha = 1f;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;
    private float fadeDuration = 1f;

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 1f);
            StartCoroutine(FadeIn());
        }
    }

    public IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(t / fadeDuration);
            SetAlpha(normalized);
            yield return null;
        }
        SetAlpha(0f);
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = Mathf.Clamp01(t / fadeDuration);
            SetAlpha(normalized);
            yield return null;
        }
        SetAlpha(1f);
    }

    void SetAlpha(float a)
    {
        fadeImage.color = new Color(0f, 0f, 0f, a);
    }
}

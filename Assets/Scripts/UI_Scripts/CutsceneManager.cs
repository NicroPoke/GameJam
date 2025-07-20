using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI dialogueText;
    public Image fadeImage;

    [Header("Images")]
    public GameObject image1;
    public GameObject image2;
    public GameObject image3;
    public GameObject image4;

    [Header("Dialogue Texts")]
    [TextArea(2, 5)]
    public string[] dialogueLines;

    private int currentLine = 0;
    private bool isTransitioning = false;

    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0);
        ShowCurrentLine();
        EnableImage(1);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTransitioning)
        {
            currentLine++;
            if (currentLine < dialogueLines.Length)
            {
                if (currentLine == 8)
                {
                    StartCoroutine(TransitionAfterImage4());
                }
                else
                {
                    ShowCurrentLine();
                    UpdateImages();
                }
            }
            else
            {
                dialogueText.text = "";
                DisableAllImages();
            }
        }
    }

    void ShowCurrentLine()
    {
        dialogueText.text = dialogueLines[currentLine];
    }

    void UpdateImages()
    {
        switch (currentLine)
        {
            case 1:
                EnableImage(2);
                break;
            case 5:
                EnableImage(3);
                break;
            case 7:
                EnableImage(4);
                break;
        }
    }

    void EnableImage(int index)
    {
        image1.SetActive(index == 1);
        image2.SetActive(index == 2);
        image3.SetActive(index == 3);
        image4.SetActive(index == 4);
    }

    void DisableAllImages()
    {
        image1.SetActive(false);
        image2.SetActive(false);
        image3.SetActive(false);
        image4.SetActive(false);
    }

    IEnumerator TransitionAfterImage4()
    {
        isTransitioning = true;
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Fade(0, 1, 1f));
        DisableAllImages();
        dialogueText.text = "";
        yield return new WaitForSeconds(0.5f);
        EnableImage(1);
        yield return StartCoroutine(Fade(1, 0, 1f));
        yield return StartCoroutine(FadeTextIn(dialogueLines[currentLine]));
        isTransitioning = false;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, to);
    }

    IEnumerator FadeTextIn(string text)
    {
        dialogueText.text = text;
        dialogueText.alpha = 0f;
        float duration = 1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            dialogueText.alpha = Mathf.Lerp(0, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        dialogueText.alpha = 1f;
    }
}

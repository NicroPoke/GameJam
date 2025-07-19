using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject door;
    private SceneFader sceneFader;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    public GameObject UI;
    public GameObject blackoutObject;

    [TextArea(5, 10)]
    public string rawText;
    public string triggerBattleAfterLine;

    private string[] lines;
    private string[] rawLines;
    private int index;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public float typingSpeed = 0.05f;

    private bool waitingForRMB = false;
    private bool waitingForLMB = false;
    private bool waitingForE = false;
    private bool waitingForBattleEnd = false;
    private float holdDuration = 2f;
    private float holdTimer = 0f;
    public float fadeDuration = 1f;

    public GameObject ALLO;
    public GameObject ALLOTWO;
    public GameObject Phone;
    public GameObject LMB;
    public GameObject RMB;
    public GameObject E;
    private bool isChangingStance;
    private bool isRotating;

    private SpriteRenderer blackoutRenderer;
    private SpriteRenderer blackoutRenderer2;
    private bool playerInsideBlackout2 = false;

    void Start()
    {
        sceneFader = FindObjectOfType<SceneFader>();
        dialoguePanel.SetActive(false);
        ParseRawText();

        if (blackoutObject != null)
        {
            blackoutRenderer = blackoutObject.GetComponent<SpriteRenderer>();
            if (blackoutRenderer != null)
            {
                Color c = blackoutRenderer.color;
                c.a = 1f;
                blackoutRenderer.color = c;
                blackoutObject.SetActive(true);
            }
        }

        StartCoroutine(BeginDialogueWithDelay());
    }

    void ParseRawText()
    {
        rawLines = rawText.Split('/');
        lines = new string[rawLines.Length];
        for (int i = 0; i < rawLines.Length; i++)
        {
            string clean = rawLines[i].Trim();
            rawLines[i] = clean;

            string formatted = ReplaceFirst(clean, "**", "<b>");
            formatted = ReplaceFirst(formatted, "**", "</b>");
            formatted = ReplaceFirst(formatted, "*", "<i>");
            formatted = ReplaceFirst(formatted, "*", "</i>");
            lines[i] = formatted;
        }
    }

    string ReplaceFirst(string text, string search, string replace)
    {
        int pos = text.IndexOf(search);
        if (pos < 0) return text;
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    IEnumerator BeginDialogueWithDelay()
    {
        yield return new WaitForSeconds(1f);
        StartDialogue();
    }

    IEnumerator EndDialogueWithDelay()
    {
        yield return StartCoroutine(sceneFader.FadeOut());
        yield return new WaitForSecondsRealtime(0.5f);
        LoadNextScene();
    }

    void Update()
    {
        if (!isChangingStance)
            StartCoroutine(ChangeStance());

        if (!isRotating)
            StartCoroutine(RotatePhoneRoutine());

        if (!isDialogueActive && !waitingForRMB && !waitingForLMB && !waitingForBattleEnd && !waitingForE) return;

        if (waitingForRMB)
        {
            if (Input.GetMouseButton(1))
            {
                holdTimer += Time.unscaledDeltaTime;
                if (holdTimer >= holdDuration)
                {
                    HideAllInputHints();
                    waitingForRMB = false;
                    holdTimer = 0f;
                    dialoguePanel.SetActive(true);
                    Time.timeScale = 0f;
                    typingCoroutine = StartCoroutine(TypeLine());
                }
            }
            else
            {
                holdTimer = 0f;
            }
            return;
        }

        if (waitingForLMB)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HideAllInputHints();
                waitingForLMB = false;
                StartCoroutine(WaitAndStartDialogueAfterLMB());
            }
            return;
        }

        if (waitingForE)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HideAllInputHints();
                waitingForE = false;
                dialoguePanel.SetActive(true);
                Time.timeScale = 0f;
                typingCoroutine = StartCoroutine(TypeLine());
            }
            return;
        }

        if (waitingForBattleEnd)
        {
            door.SetActive(false);
            if (GhostManager.Instance != null && GhostManager.Instance.battleEnded)
            {
                waitingForBattleEnd = false;
                dialoguePanel.SetActive(true);
                Time.timeScale = 0f;
                NextLine();
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }

        UI.SetActive(waitingForRMB || waitingForLMB || waitingForE || waitingForBattleEnd);
    }

    void HideAllInputHints()
    {
        if (RMB != null) RMB.SetActive(false);
        if (LMB != null) LMB.SetActive(false);
        if (E != null) E.SetActive(false);
    }

    IEnumerator ChangeStance()
    {
        isChangingStance = true;
        ALLO.SetActive(true);
        ALLOTWO.SetActive(false);
        yield return new WaitForSecondsRealtime(0.2f);
        ALLO.SetActive(false);
        ALLOTWO.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        isChangingStance = false;
    }

    IEnumerator RotatePhoneRoutine()
    {
        if (Phone == null) yield break;
        isRotating = true;
        Phone.transform.rotation = Quaternion.Euler(0, 0, -2f);
        yield return new WaitForSecondsRealtime(0.4f);
        Phone.transform.rotation = Quaternion.Euler(0, 0, 0f);
        yield return new WaitForSecondsRealtime(0.4f);
        isRotating = false;
    }

    IEnumerator WaitAndStartDialogueAfterLMB()
    {
        LMB.SetActive(false);
        yield return new WaitForSecondsRealtime(2f);
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    void StartDialogue()
    {
        index = 0;
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        Time.timeScale = 0f;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        index++;
        if (index < lines.Length)
        {
            string currentRawLine = rawLines[index].Trim();

            if (!string.IsNullOrEmpty(triggerBattleAfterLine) && currentRawLine.Equals(triggerBattleAfterLine.Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;
                StartCoroutine(FadeOutBlackout());
                waitingForBattleEnd = true;
                return;
            }

            if (index == 7 && SceneManager.GetActiveScene().buildIndex == 1)
            {
                dialoguePanel.SetActive(false);
                RMB.SetActive(true);
                Time.timeScale = 1f;
                waitingForRMB = true;
                return;
            }

            if (index == 10 && SceneManager.GetActiveScene().buildIndex == 1)
            {
                dialoguePanel.SetActive(false);
                LMB.SetActive(true);
                Time.timeScale = 1f;
                waitingForLMB = true;
                return;
            }

            if (index == 6 && SceneManager.GetActiveScene().buildIndex == 6)
            {
                dialoguePanel.SetActive(false);
                E.SetActive(true);
                Time.timeScale = 1f;
                waitingForE = true;
                return;
            }

            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            Time.timeScale = 1f;
            StartCoroutine(EndDialogueWithDelay());
        }
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Last scene.");
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in lines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    IEnumerator FadeOutBlackout()
    {
        if (blackoutRenderer == null) yield break;

        float elapsed = 0f;
        Color startColor = blackoutRenderer.color;
        Color endColor = startColor;
        endColor.a = 0f;

        while (elapsed < fadeDuration)
        {
            blackoutRenderer.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        blackoutRenderer.color = endColor;
        blackoutObject.SetActive(false);
    }
}

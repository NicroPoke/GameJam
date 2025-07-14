using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    private SceneFader sceneFader;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] lines;
    public string triggerBattleAfterLine;

    private int index;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public float typingSpeed = 0.05f;

    private bool waitingForRMB = false;
    private bool waitingForLMB = false;
    private bool waitingForBattleEnd = false;
    private float holdDuration = 2f;
    private float holdTimer = 0f;

    void Start()
    {
        sceneFader = FindObjectOfType<SceneFader>();
        dialoguePanel.SetActive(false);
        StartCoroutine(BeginDialogueWithDelay());
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
        if (!isDialogueActive && !waitingForRMB && !waitingForLMB && !waitingForBattleEnd) return;

        if (waitingForRMB)
        {
            if (Input.GetMouseButton(1))
            {
                holdTimer += Time.unscaledDeltaTime;
                if (holdTimer >= holdDuration)
                {
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
                waitingForLMB = false;
                dialoguePanel.SetActive(true);
                Time.timeScale = 0f;
                typingCoroutine = StartCoroutine(TypeLine());
            }
            return;
        }

        if (waitingForBattleEnd)
        {
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
            string currentLine = lines[index];

            if (!string.IsNullOrEmpty(triggerBattleAfterLine) && currentLine == triggerBattleAfterLine)
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;
                waitingForBattleEnd = true;
                return;
            }

            if (index == 6 && SceneManager.GetActiveScene().buildIndex == 1)
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;
                waitingForRMB = true;
                return;
            }

            if (index == 7 && SceneManager.GetActiveScene().buildIndex == 1)
            {
                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;
                waitingForLMB = true;
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
            Debug.Log("Последняя сцена достигнута.");
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
}

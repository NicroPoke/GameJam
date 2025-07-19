using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI dialogueText;

    [Header("Images")]
    public GameObject image1;
    public GameObject image2;
    public GameObject image3;
    public GameObject image4;

    [Header("Dialogue Texts")]
    [TextArea(2, 5)]
    public string[] dialogueLines;

    private int currentLine = 0;

    void Start()
    {
        ShowCurrentLine();
        EnableImage(1); 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentLine++;
            if (currentLine < dialogueLines.Length)
            {
                ShowCurrentLine();
                UpdateImages();
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
}

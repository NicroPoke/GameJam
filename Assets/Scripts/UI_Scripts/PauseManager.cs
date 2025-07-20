using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject RetryCanvas;
    public GameObject Canvas;
    public MonoBehaviour[] scriptsToDisable;
    private bool isPaused = false;
    private bool retryTriggered = false;
    public float retryDelay = 2f; 

    void Start()
    {
        Time.timeScale = 1f;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (player != null && player.isDead && !RetryCanvas.activeSelf && !retryTriggered)
        {
            retryTriggered = true;
            StartCoroutine(ShowRetryCanvasAfterDelay());
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Presses");
            isPaused = !isPaused;
            Canvas.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;

            foreach (var script in scriptsToDisable)
                script.enabled = !isPaused;
        }
    }

    IEnumerator ShowRetryCanvasAfterDelay()
    {
        yield return new WaitForSecondsRealtime(retryDelay); 
        Debug.Log("Retried");

        RetryCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;
        retryTriggered = false;
        Canvas.SetActive(false);
        RetryCanvas.SetActive(false);
        foreach (var script in scriptsToDisable)
            script.enabled = true;
    }
}

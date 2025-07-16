using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject Canvas;
    public MonoBehaviour[] scriptsToDisable;
    private bool isPaused = false;

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;
        Canvas.SetActive(false);
        foreach (var script in scriptsToDisable)
            script.enabled = true;
    }
}

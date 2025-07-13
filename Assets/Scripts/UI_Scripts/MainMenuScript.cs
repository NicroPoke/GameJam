using System;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
public class MainMenuScript : MonoBehaviour
{

    public void LoadTutorial(){
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitAplication(){
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
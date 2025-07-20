using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class MainMenuScript : MonoBehaviour
{

    public void LoadTutorial()
    {
        SceneManager.LoadScene("Day1");
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitAplication()
    {
        Application.Quit();

    }

    public void ChangeIcon(Sprite icon)
    {
        Debug.Log("Triggered");
        GetComponent<UnityEngine.UI.Image>().sprite = icon;
    }
}
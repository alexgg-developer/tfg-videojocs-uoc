using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt("Turns", 60);
        PlayerPrefs.SetInt("IAPlayer", 1);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        PlayerPrefs.SetInt("SceneToLoad", 2);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }


    public void LoadEditor()
    {
        SceneManager.LoadScene(1);
        PlayerPrefs.SetInt("SceneToLoad", 3);
    }
}

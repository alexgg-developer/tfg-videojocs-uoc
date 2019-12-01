using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI victoryMessageText;

    Logic logic = null;

    private void Awake()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
    }

    private void OnEnable()
    {
        if(logic.IsEndOfGame) {
            int playerNumber = logic.GetScorePlayerWinner() + 1;
            victoryMessageText.text = "It's the end of times and Player " + playerNumber + " has won.";
        }
        else {
            int playerNumber = logic.GetLastCivilizationStanding() + 1;
            victoryMessageText.text = "It's the end of times and Player " + playerNumber + " is the last civilization standing.";
        }

    }

    public void GoBackToMenu()
    {
        PlayerPrefs.SetInt("SceneToLoad", 0);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

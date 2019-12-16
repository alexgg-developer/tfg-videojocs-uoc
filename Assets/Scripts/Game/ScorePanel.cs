using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    Text[] playerTexts;
    [SerializeField]
    Text scoreTurnText;
#pragma warning restore 0649
    Logic logic;
    int[] lastScore;
    //int turn = 1;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        for(int i = 0; i < logic.NumberPlayers; ++i) {
            playerTexts[i].gameObject.SetActive(true);
        }
        lastScore = new int[logic.NumberPlayers];
        for (int i = 0; i < logic.NumberPlayers; ++i) {
            lastScore[i] = -1;
        }
        playerTexts[logic.CurrentPlayer].fontSize = 14;
    }

    public void OnChangeOfTurn(int newTurn)
    {
        scoreTurnText.text = "Scores - Turn " + newTurn;
        for(int i = 0; i < logic.NumberPlayers; ++i) {
            //playerTexts[i].fontStyle = FontStyle.Normal;
            playerTexts[logic.CurrentPlayer].fontSize = 12;
        }
        //playerTexts[logic.CurrentPlayer].fontStyle = FontStyle.Bold;
        playerTexts[logic.CurrentPlayer].fontSize = 14;
    }

    public void OnChangeOfPlayer(int newPlayerID)
    {
        for (int i = 0; i < logic.NumberPlayers; ++i) {
            //playerTexts[i].fontStyle = FontStyle.Normal;
            playerTexts[i].fontSize = 12;
        }
        //playerTexts[logic.CurrentPlayer].fontStyle = FontStyle.Bold;
        playerTexts[newPlayerID].fontSize = 14;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < logic.NumberPlayers; ++i) {
            if (lastScore[i] != logic.Players[i].Score) {
                if (logic.IsThereAI && i == 1) {
                    playerTexts[i].text = "(AI)Player " + (i + 1) + ": " + logic.Players[i].Score;
                }
                else {
                    playerTexts[i].text = "Player " + (i + 1) + ": " + logic.Players[i].Score;
                }
            }
        }
    }
}

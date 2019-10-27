using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    Text[] playerTexts;
    Logic logic;
    int[] lastScore;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        for(int i = 0; i < logic.NumberPlayers; ++i) {
            playerTexts[i].gameObject.SetActive(true);
        }
        lastScore = new int[logic.NumberPlayers];
        for (int i = 0; i < logic.NumberPlayers; ++i) {
            lastScore[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < logic.NumberPlayers; ++i) {
            if (lastScore[i] != logic.Players[i].Score) {
                playerTexts[i].text = "Player " + (i+1) + ": " + logic.Players[i].Score;
            }
        }
    }
}

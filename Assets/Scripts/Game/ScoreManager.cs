using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{
    public enum TypesScore { CITY, FIGHT, FIGHT_CITY, TECHNOLOGY, RESOURCE, SIZE }
    int[] scores;

    Player[] players;
    // Start is called before the first frame update
    public ScoreManager(Player[] givenPlayers)
    {
        players = givenPlayers;
        scores = new int[(int)TypesScore.SIZE];
        scores[0] = 100;
        scores[1] = 10;
        scores[2] = 300;
        scores[3] = 500;
        scores[4] = 200;
    }

    public void OnNewCity(int playerID)
    {
        players[playerID].Score += scores[(int)TypesScore.CITY];
    }

    public void OnFightWon(int playerID)
    {
        players[playerID].Score += scores[(int)TypesScore.FIGHT];
    }

    public void OnScoreEvent(TypesScore type, int playerID)
    {
        players[playerID].Score += scores[(int)type];
    }

    internal int GetScorePlayerWinner(int numberPlayers)
    {
        int playerID = 0;
        int greatestScore = players[0].Score;
        for(int i = 1; i < numberPlayers; ++i) {
            if(players[i].Score > greatestScore) {
                playerID = i;
            }
        }

        return playerID;
    }
}

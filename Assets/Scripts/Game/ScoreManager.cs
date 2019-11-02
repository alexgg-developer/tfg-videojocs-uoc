﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{
    public enum TypesScore { CITY, FIGHT, SIZE }
    int[] scores;

    Player[] players;
    // Start is called before the first frame update
    public ScoreManager(Player[] givenPlayers)
    {
        players = givenPlayers;
        scores = new int[(int)TypesScore.SIZE];
        scores[0] = 100;
        scores[1] = 10;
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
}

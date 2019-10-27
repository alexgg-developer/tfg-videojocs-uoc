using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private RandomHexGridCreator gridCreator;
    private Player[] players;
    public Player[] Players { get { return players; }}
    private Tuple<int, int>[] initialPositions;
    int numberPlayers = 2;
    public int NumberPlayers { get { return numberPlayers; } set { numberPlayers = value; } }
    // Start is called before the first frame update
    void Awake()
    {
        //TO DO:
		//numberPlayers = PlayerPrefs.GetInt("NumberPlayers");
        numberPlayers = 2;
        players = new Player[numberPlayers];
        initialPositions = new Tuple<int, int>[numberPlayers];
        initialPositions[0] = new Tuple<int,int>(0, 0);
        //initialPositions[1] = new Tuple<int, int>(gridCreator.Width - 2, gridCreator.Height - 2);
        initialPositions[1] = new Tuple<int, int>(gridCreator.Width - 1, gridCreator.Height - 2);
        gridCreator.CreateGrid();
        for (int i = 0; i < numberPlayers; ++i) {
            UnitManager unitManager = Instantiate(Resources.Load("UnitManager", typeof(UnitManager))) as UnitManager;
            players[i] = new Player(unitManager);
            players[i].SetGrid(gridCreator.HexGrid);
            players[i].InstantiateIntialUnits(initialPositions[i]);
        }
        //unitManager.InstantiateIntialUnits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

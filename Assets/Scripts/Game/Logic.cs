using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ScoreManager;

[System.Serializable] public class UnityIntEvent : UnityEvent<int> { }

public class Logic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private RandomHexGridCreator gridCreator;
    [SerializeField]
    UnityIntEvent changeOfTurnEvent;
    [SerializeField]
    UnityIntEvent changeOfPlayerEvent;

    public ScoreManager scoreManager;
#pragma warning restore 0649
    private Player[] players;
    public Player[] Players { get { return players; }}
    private Tuple<int, int>[] initialPositions;
    int numberPlayers = 2;
    public int NumberPlayers { get { return numberPlayers; } set { numberPlayers = value; } }
    int turn = 1;
    public int Turn { get { return turn; } set { turn = value; } }
    int currentPlayer = 0;
    public int CurrentPlayer { get { return currentPlayer; } set { currentPlayer = value; } }
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
        //initialPositions[1] = new Tuple<int, int>(gridCreator.Width - 1, gridCreator.Height - 2);
        initialPositions[1] = new Tuple<int, int>(1, 1);
        gridCreator.CreateGrid();
        for (int i = 0; i < numberPlayers; ++i) {
            UnitManager unitManager = Instantiate(Resources.Load("UnitManager", typeof(UnitManager))) as UnitManager;
            unitManager.PlayerID = i;
            CityManager cityManager = Instantiate(Resources.Load("CityManager", typeof(CityManager))) as CityManager;
            cityManager.PlayerID = i;   
            players[i] = new Player(unitManager, cityManager);
            players[i].SetGrid(gridCreator.HexGrid);
            players[i].InstantiateIntialUnits(initialPositions[i]);
        }
        //unitManager.InstantiateIntialUnits();
        scoreManager = new ScoreManager(players);
        //scoreEvent.AddListener(scoreManager.OnScoreEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            players[currentPlayer].ResetUnitMovement();
            ++currentPlayer;
            if (currentPlayer == numberPlayers) {
                currentPlayer = 0;
                ++turn;
                changeOfTurnEvent.Invoke(turn);
            }
            changeOfPlayerEvent.Invoke(currentPlayer);
        }
    }

    public bool BuildCity(HexCoordinates hexCoordinates)
    {
        if(IsCellFree(hexCoordinates)) {
            players[currentPlayer].AddCity(hexCoordinates);
            scoreManager.OnScoreEvent(TypesScore.CITY, currentPlayer);
            return true;
        }

        return false;
    }

    public void RemoveUnit(Unit unit)
    {
        players[unit.PlayerID].RemoveUnit(unit);
    }

    private bool IsCellFree(HexCoordinates hexCoordinates)
    {
        /*bool isFree = true;
        for(int i = 0; i < numberPlayers && isFree; ++i) {
            isFree = IsThereCityAt(hexCoordinates);
        }

        return isFree;*/
        return !IsThereCityAt(hexCoordinates);
    }

    private bool IsThereCityAt(HexCoordinates hexCoordinates)
    {
        HexCell cell = gridCreator.GetCell(hexCoordinates);
        City city = cell.GetComponentInChildren<City>();

        return city != null;
    }

    public void OnScoreEvent(TypesScore type, int playerID)
    {
        scoreManager.OnScoreEvent(type, playerID);
    }
}

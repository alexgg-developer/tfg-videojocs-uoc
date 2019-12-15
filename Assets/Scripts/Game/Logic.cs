using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ScoreManager;
using static TechnologyInfo;

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
    [SerializeField]
    GameObject victoryPanel;

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
    bool isEndOfGame = false;
    public bool IsEndOfGame { get { return isEndOfGame; } set { isEndOfGame = value; } }
    int turnsToPlay = 10;
    public int TurnsToPlay { get { return turnsToPlay; } set { turnsToPlay = value; } }

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
            TechnologyManager technologyManager = new TechnologyManager();

            players[i] = new Player(unitManager, cityManager, technologyManager);
            players[i].SetGrid(gridCreator.HexGrid);
            players[i].InstantiateIntialUnits(initialPositions[i]);
        }
        //unitManager.InstantiateIntialUnits();
        scoreManager = new ScoreManager(players);
        //scoreEvent.AddListener(scoreManager.OnScoreEvent);
    }

    internal void AddTechnology(TechnologyType technologyType)
    {
        players[currentPlayer].AddTechnology(technologyType);
        OnScoreEvent(TypesScore.TECHNOLOGY, currentPlayer);
    }

    private void Start()
    {
        changeOfPlayerEvent.Invoke(currentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            do {
                ++currentPlayer;
                if (currentPlayer == numberPlayers) {
                    currentPlayer = 0;
                    ++turn;
                    changeOfTurnEvent.Invoke(turn);
                    isEndOfGame = IsEndOfGameTurn();
                }
                isEndOfGame = isEndOfGame || CheckSurvivalEndOfGameTurn();
                if (!isEndOfGame) {
                    players[currentPlayer].ResetUnitMovement();
                    players[currentPlayer].ProduceShields();
                    changeOfPlayerEvent.Invoke(currentPlayer);
                    players[currentPlayer].ProduceShields();
                }
                else {
                    victoryPanel.SetActive(true);
                    break;
                }
            } while (players[currentPlayer].IsDead);
        }
    }

    internal bool IsEndOfGameTurn()
    {
        return turnsToPlay == turn;
    }

    internal bool HasTechnology(TechnologyType technologyType)
    {
        return players[currentPlayer].HasTechnology(technologyType);
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

    internal bool TryBuildingUnit(UnitStats.UnitType unitType, City city)
    {
        var hexCoordinates = city.gameObject.GetComponentInParent<HexCell>().coordinates;
        if(!IsThereUnit(hexCoordinates)) {
            players[currentPlayer].AddUnit(unitType, hexCoordinates);
            return true;
        }
        return false;
    }

    private bool IsThereUnit(HexCoordinates hexCoordinates)
    {
        HexCell cell = gridCreator.GetCell(hexCoordinates);
        Unit unit = cell.GetComponentInChildren<Unit>();

        return unit != null;
    }

    public void RemoveUnit(Unit unit)
    {
        players[unit.PlayerID].RemoveUnit(unit);
    }

    private bool IsCellFree(HexCoordinates hexCoordinates)
    {
        return !IsThereCityAt(hexCoordinates);
    }

    internal bool TrySpendShields(uint shieldCost)
    {
        if(players[currentPlayer].Shields >= shieldCost) {
            players[currentPlayer].Shields -= shieldCost;
            return true;
        }
        return false;
    }

    internal bool IsThereEnoughShields(uint shieldCost)
    {
       return players[currentPlayer].Shields >= shieldCost;
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

    internal void TransferCity(int defeatedPlayerID, int conquererPlayerID, int cityID)
    {
        City city = players[defeatedPlayerID].RemoveCity(cityID);
        city.PlayerID = conquererPlayerID;
        players[conquererPlayerID].TransferCity(city);
    }


    internal int GetScorePlayerWinner()
    {
        return scoreManager.GetScorePlayerWinner(numberPlayers);
    }


    internal int GetLastCivilizationStanding()
    {
        for (int i = 0; i < numberPlayers; ++i) {
            if(!players[i].IsDead) {
                return i;
            }
        }
        return -1;
    }
    
    private bool CheckSurvivalEndOfGameTurn()
    {
        int numberOfPlayersAlive = 0;

        for (int i = 0; i < numberPlayers; ++i) {
            if (!players[i].CheckDeath()) {
                ++numberOfPlayersAlive;
            }
        }

        return numberOfPlayersAlive == 1;
    }

    public City FindNearestCity(HexCoordinates resourceCoordinates)
    {
        var city = players[currentPlayer].FindNearestCity(resourceCoordinates);

        return city;
    }
}

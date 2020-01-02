using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BuildingInfo;
using static ScoreManager;
using static TechnologyInfo;
using static UnitStats;

[System.Serializable] public class UnityIntEvent : UnityEvent<int> { }

public class Logic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    //private RandomHexGridCreator gridCreator;
    private HexGridLoader gridCreator;
    [SerializeField]
    UnityIntEvent changeOfTurnEvent;
    [SerializeField]
    UnityIntEvent changeOfPlayerEvent;
    [SerializeField]
    GameObject victoryPanel;
    [SerializeField]
    UnitController unitController;
    [SerializeField]
    UnitStats[] unitStats;
    [SerializeField]
    BuildingInfo[] buildingInfo;
    [SerializeField]
    TechnologyInfo[] technologyInfo;
    [SerializeField]
    private AnimationController animationController;
#pragma warning restore 0649
    public ScoreManager scoreManager;

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
    int turnsToPlay = 200;
    public int TurnsToPlay { get { return turnsToPlay; } set { turnsToPlay = value; } }
    bool isThereAI = true;
    public bool IsThereAI { get { return isThereAI; } set { isThereAI = value; } }
    bool isCurrentPlayerAI  = false;
    public bool IsCurrentPlayerAI { get { return isCurrentPlayerAI; } set { isCurrentPlayerAI = value; } }

    public bool IsAnimationOn { get { return animationController.IsAnimationOn; } }


    /*private Unit lastUnitBuilt = null;
public Unit LastUnitBuilt { get { return lastUnitBuilt; } set { lastUnitBuilt = value; } }*/

    AIPlayer aiPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        //numberPlayers = PlayerPrefs.GetInt("NumberPlayers");
        isThereAI = PlayerPrefs.GetInt("IAPlayer") > 0;
        turnsToPlay = PlayerPrefs.GetInt("Turns");
        numberPlayers = 2;
        players = new Player[numberPlayers];
        initialPositions = new Tuple<int, int>[numberPlayers];
        initialPositions[0] = new Tuple<int,int>(0, 0);
        //initialPositions[1] = new Tuple<int, int>(gridCreator.Width - 2, gridCreator.Height - 2);
        //initialPositions[1] = new Tuple<int, int>(gridCreator.Width - 1, gridCreator.Height - 2);
        initialPositions[1] = new Tuple<int, int>(9, 9);
        //initialPositions[1] = new Tuple<int, int>(1, 1);
        gridCreator.CreateGrid();
        for (int i = 0; i < numberPlayers; ++i) {
            UnitManager unitManager = Instantiate(Resources.Load("UnitManager", typeof(UnitManager))) as UnitManager;
            unitManager.PlayerID = i;
            CityManager cityManager = Instantiate(Resources.Load("CityManager", typeof(CityManager))) as CityManager;
            cityManager.PlayerID = i;
            TechnologyManager technologyManager = new TechnologyManager();

            players[i] = new Player(unitManager, cityManager, technologyManager);
            players[i].SetGrid(gridCreator.HexGrid);
            players[i].InstantiateInitialUnits(initialPositions[i]);
        }
        //unitManager.InstantiateIntialUnits();
        scoreManager = new ScoreManager(players);
        //scoreEvent.AddListener(scoreManager.OnScoreEvent);
        if (IsThereAI) {
            aiPlayer = new AIPlayer(players[1], this, unitStats);
        }
    }

    internal List<BuildingType> GetBuildingsAvailable(City selectedCity)
    {
        List<BuildingType> buildingsAvailable = new List<BuildingType>();

        if (HasTechnology(TechnologyType.AGRICULTURE)) {
            buildingsAvailable.Add(BuildingType.BARN);
        }
        if (HasTechnology(TechnologyType.NAVIGATION)) {
            buildingsAvailable.Add(BuildingType.DOCK);
        }
        if (HasTechnology(TechnologyType.ENGINEERING)) {
            buildingsAvailable.Add(BuildingType.WALL);
        }

        return buildingsAvailable;
    }

    internal uint GetCostBuilding(BuildingType buildingType)
    {
        return buildingInfo[(int)buildingType].ShieldCost;
    }

    internal List<UnitType> GetUnitsAvailable(bool hasAccesToWater)
    {
        List<UnitType> unitsAvailable = new List<UnitStats.UnitType>();

        unitsAvailable.Add(UnitType.WARRIOR);
        unitsAvailable.Add(UnitType.SETTLER);
        if (HasTechnology(TechnologyType.AGRICULTURE)) {
            unitsAvailable.Add(UnitType.HORSEMAN);
        }
        if (HasTechnology(TechnologyType.NAVIGATION) && hasAccesToWater) {
            unitsAvailable.Add(UnitType.SHIP);
        }
        if (HasTechnology(TechnologyType.ENGINEERING)) {
            unitsAvailable.Add(UnitType.CATAPULT);
            unitsAvailable.Add(UnitType.ARCHER);
        }

        return unitsAvailable;
    }

    internal uint GetCostTechnology(TechnologyType technologyType)
    {
        return technologyInfo[(int)technologyType].ShieldCost;
    }

    internal List<TechnologyType> GetTechnologiesLeft()
    {
        List<TechnologyType> technologiesLeft = new List<TechnologyType>();
        if (!HasTechnology(TechnologyType.AGRICULTURE)) {
            technologiesLeft.Add(TechnologyType.AGRICULTURE);
        }
        if (!HasTechnology(TechnologyType.NAVIGATION)) {
            technologiesLeft.Add(TechnologyType.NAVIGATION);
        }
        if (!HasTechnology(TechnologyType.ENGINEERING)) {
            technologiesLeft.Add(TechnologyType.ENGINEERING);
        }

        return technologiesLeft;
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
        if (isEndOfGame) return;

        if (!IsAnimationOn) {
            if (IsCurrentPlayerAI) {
                aiPlayer.NextTask();
                if (!aiPlayer.IsThereTasks()) {
                    NextPlayer();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space)) {
                NextPlayer();
            }
        }
    }

    void NextPlayer()
    {
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
                IsCurrentPlayerAI = isThereAI && currentPlayer == 1;
                if (IsCurrentPlayerAI) aiPlayer.InitialTasks();
            }
            else {
                victoryPanel.SetActive(true);
                break;
            }
        } while (players[currentPlayer].IsDead);
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
        var cell = city.gameObject.GetComponentInParent<HexCell>();
        var hexCoordinates = cell.coordinates;
        if(!IsThereUnit(hexCoordinates)) {
            var lastUnitBuilt = players[currentPlayer].AddUnit(unitType, cell);
            unitController.Select(cell, lastUnitBuilt);
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
        Destroy(unit.gameObject);
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


    internal List<City> GetCities()
    {
        return players[currentPlayer].GetCities();
    }
}

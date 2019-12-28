using System;
using System.Collections.Generic;
using UnityEngine;
using static BuildingInfo;
using static TechnologyInfo;
using static UnitStats;

public class AIPlayer
{
    Player player;
    private Logic logic;
    private UnitController unitController;
    UnitStats[] unitStats;

    //HexGrid grid;
    //int currentTask = 0;
    Queue<Action> tasks = new Queue<Action>();
    enum ScheduledTask { NONE, BUILD_UNIT, BUILD_BUILDING, INVESTIGATE };
    ScheduledTask currentScheduledTask = ScheduledTask.NONE;

    public AIPlayer(Player playerToControl, Logic givenLogic, UnitStats[] givenUnitStats)
    {
        player = playerToControl;
        logic = givenLogic;
        unitController = logic.GetComponent<UnitController>();
        unitStats = givenUnitStats;
    }

    internal void InitialTasks()
    {
        tasks.Enqueue(new Action(TryBuildingCity));
        tasks.Enqueue(new Action(TryAttack));
        tasks.Enqueue(new Action(MoveUnits));
        tasks.Enqueue(new Action(ScheduleTask));

    }

    private void ScheduleTask()
    {
        Debug.Log("Turn: " + logic.Turn.ToString());
        if (currentScheduledTask == ScheduledTask.NONE) {
            float random = UnityEngine.Random.value;
            if (random < 0.33f) {
                currentScheduledTask = ScheduledTask.BUILD_UNIT;
            }
            else if (random < 0.67f) {
                currentScheduledTask = ScheduledTask.BUILD_BUILDING;
            }
            else {
                currentScheduledTask = ScheduledTask.INVESTIGATE;
            }
            Debug.Log("ScheduleTask: " + currentScheduledTask.ToString());
        }
        else {
            switch(currentScheduledTask) {
                case ScheduledTask.BUILD_UNIT: {
                        Debug.Log("Try building unit");
                        List<City> cities = logic.GetCities();
                        if (cities.Count > 0) {
                            Debug.Log("More than one city");
                            List<UnitType> unitsAvailable = logic.GetUnitsAvailable();
                            int unitChosen = UnityEngine.Random.Range(0, unitsAvailable.Count);
                            int cityChosen = UnityEngine.Random.Range(0, cities.Count);
                            if (logic.IsThereEnoughShields(unitStats[(int)unitsAvailable[unitChosen]].ShieldCost)) {
                                if (logic.TryBuildingUnit(unitsAvailable[unitChosen], cities[cityChosen])) {
                                    logic.TrySpendShields(unitStats[(int)unitChosen].ShieldCost);
                                    currentScheduledTask = ScheduledTask.NONE;
                                    tasks.Enqueue(new Action(ScheduleTask));
                                }
                            }
                        }
                    }
                    break;
                case ScheduledTask.BUILD_BUILDING: {
                        List<City> cities = logic.GetCities();
                        if (cities.Count > 0) {
                            int cityChosen = UnityEngine.Random.Range(0, cities.Count);
                            City selectedCity = cities[cityChosen];
                            List<BuildingType> buildingsAvailable = logic.GetBuildingsAvailable(selectedCity);
                            if (buildingsAvailable.Count > 0) {
                                int buildingChosen = UnityEngine.Random.Range(0, buildingsAvailable.Count);
                                var buildingType = buildingsAvailable[buildingChosen];
                                if (!selectedCity.HasBuilding(buildingType)) {
                                    var shieldCost = logic.GetCostBuilding(buildingType);
                                    if (logic.TrySpendShields(shieldCost)) {
                                        selectedCity.BuildBuilding(buildingType);
                                        currentScheduledTask = ScheduledTask.NONE;
                                        tasks.Enqueue(new Action(ScheduleTask));
                                    }
                                }
                            }
                            else {
                                currentScheduledTask = ScheduledTask.NONE;
                                tasks.Enqueue(new Action(ScheduleTask));
                            }
                        }

                    }
                    break;
                case ScheduledTask.INVESTIGATE:
                    List<TechnologyType> technologiesLeft = logic.GetTechnologiesLeft();
                    if (technologiesLeft.Count > 0) {
                        int technologyChosen = UnityEngine.Random.Range(0, technologiesLeft.Count);
                        var technologyType = technologiesLeft[technologyChosen];
                        var shieldCost = logic.GetCostTechnology(technologyType);
                        if (logic.IsThereEnoughShields(shieldCost)) {
                            logic.AddTechnology(technologyType);
                            logic.TrySpendShields(shieldCost);
                            currentScheduledTask = ScheduledTask.NONE;
                            tasks.Enqueue(new Action(ScheduleTask));
                        }
                    }
                    else {
                        currentScheduledTask = ScheduledTask.NONE;
                        tasks.Enqueue(new Action(ScheduleTask));
                    }
                    break;
            }
        }
        Debug.Log("end Turn: " + logic.Turn.ToString());
    }

    internal void NextTask()
    {
        if (IsThereTasks()) {
            var task = tasks.Dequeue();
            task();
        }
    }

    public bool IsThereTasks()
    {
        return tasks.Count > 0;
    }

    void MoveUnits()
    {
        var units = player.GetUnits();
        foreach (GameObject unit in units) {
            Unit unitComponent = unit.GetComponent<Unit>();
            const uint MAX_TRIES = 6;
            uint t = 0;
            while (unitComponent.MovementLeft > 0 && t <= MAX_TRIES) {
                ++t;
                var cell = unit.transform.parent.GetComponent<HexCell>();
                var neighborCell = cell.GetRandomNeighbor();
                if (neighborCell != null) {
                    var cityInGoalCell = neighborCell.gameObject.GetComponentInChildren<City>();
                    var unitInGoalCell = neighborCell.gameObject.GetComponentInChildren<Unit>();
                    if (cityInGoalCell == null && unitInGoalCell == null) {
                        MoveUnit(unit, neighborCell);
                        if (unitComponent.Type == UnitStats.UnitType.SETTLER) {
                            tasks.Enqueue(new Action(TryBuildingCity));
                        }
                    }
                }
                else {
                    continue;
                }
            }
        }
    }

    void TryAttack()
    {
        var units = player.GetUnits();
        for (int i = units.Count - 1; i >= 0; --i) {
            var unit = units[i];
            Unit unitComponent = unit.GetComponent<Unit>();
            if (unitComponent.Type != UnitStats.UnitType.SETTLER) {
                var cell = unit.transform.parent.GetComponent<HexCell>();
                var cellToAttack = CanAttackUnit(cell);
                if (cellToAttack != null) {
                    var unitInGoalCell = cellToAttack.gameObject.GetComponentInChildren<Unit>();
                    var cityInGoalCell = cellToAttack.gameObject.GetComponentInChildren<City>();
                    unitController.FightLogic(unitComponent, unitInGoalCell, cityInGoalCell != null, cellToAttack, cityInGoalCell);
                }
            }
        }
    }

    void TryBuildingCity()
    {
        var units = player.GetUnits();
        for (int i = units.Count - 1; i >= 0; --i) {
            var unit = units[i];
            Unit unitComponent = unit.GetComponent<Unit>();
            if (unitComponent.Type == UnitStats.UnitType.SETTLER) {
                var cell = unit.transform.parent.GetComponent<HexCell>();
                if (CanBuildCity(cell)) {
                    SettlerCommand(unitComponent, cell);
                }
            }
        }
    }

    public void SettlerCommand(Unit settler, HexCell cell)
    {
        if (logic.BuildCity(cell.coordinates)) {
            logic.RemoveUnit(settler);
            cell.SetResource(false);
        }
    }

    HexCell CanAttackUnit(HexCell cell)
    {
        var neighbors = cell.GetNeighbors();
        HexCell cellToAttack = null;

        foreach (var neighbor in neighbors) {
            if (neighbor != null) {
                var cityInGoalCell = neighbor.gameObject.GetComponentInChildren<City>();
                if (cityInGoalCell != null && cityInGoalCell.PlayerID != player.PlayerID) {
                    cellToAttack = cityInGoalCell.transform.parent.GetComponent<HexCell>();
                    break;
                }
            }
        }

        if (cellToAttack == null) {
            foreach (var neighbor in neighbors) {
                if (neighbor != null) {
                    var unitInGoalCell = neighbor.gameObject.GetComponentInChildren<Unit>();
                    if (unitInGoalCell != null && unitInGoalCell.PlayerID != player.PlayerID) {
                        cellToAttack = unitInGoalCell.transform.parent.GetComponent<HexCell>();
                        break;
                    }
                }
            }
        }

        return cellToAttack;
    }

    bool CanBuildCity(HexCell cell)
    {
        var neighbors = cell.GetNeighbors();
        bool isThereCityClose = false;
        bool isThereEnemyClose = false;

        foreach (var neighbor in neighbors) {
            if (neighbor != null) {
                var cityInGoalCell = neighbor.gameObject.GetComponentInChildren<City>();
                var unitInGoalCell = neighbor.gameObject.GetComponentInChildren<Unit>();
                isThereCityClose = isThereCityClose || cityInGoalCell != null;
                isThereEnemyClose = isThereEnemyClose || unitInGoalCell != null && unitInGoalCell.PlayerID != player.PlayerID;
            }
        }

        return !isThereCityClose && !isThereEnemyClose;
    }

    void MoveUnit(GameObject unitToMove, HexCell cellToMoveTo)
    {
        unitToMove.transform.SetParent(cellToMoveTo.transform);
        float offsetY = unitToMove.GetComponent<MeshFilter>().mesh.bounds.size.y * unitToMove.transform.localScale.y * 0.5f;
        unitToMove.transform.localPosition = new Vector3(0f, offsetY, 0f);
        var unit = unitToMove.GetComponent<Unit>();
        --unit.MovementLeft;
        unit.OnNewPosition();
    }
}

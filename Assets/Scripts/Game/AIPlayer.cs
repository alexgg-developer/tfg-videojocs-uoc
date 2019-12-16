using System;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer
{
    Player player;
    private Logic logic;
    private UnitController unitController;

    //HexGrid grid;
    //int currentTask = 0;
    Queue<Action> tasks = new Queue<Action>();

    public AIPlayer(Player playerToControl, Logic givenLogic)
    {
        player = playerToControl;
        logic = givenLogic;
        unitController = logic.GetComponent<UnitController>();
    }

    internal void InitialTasks()
    {
        tasks.Enqueue(new Action(TryBuildingCity));
        tasks.Enqueue(new Action(TryAttack));
        tasks.Enqueue(new Action(MoveUnits));
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
            while (unitComponent.MovementLeft > 0) {
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
                var unitInGoalCell = neighbor.gameObject.GetComponentInChildren<Unit>();
                if(unitInGoalCell != null && unitInGoalCell.PlayerID != player.PlayerID) {
                    cellToAttack = unitInGoalCell.transform.parent.GetComponent<HexCell>();
                    break;
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

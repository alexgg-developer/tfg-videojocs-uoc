using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    UnitManager unitManager;
    int score = 0;
    public int Score { get { return score; } set { score = value; } }


    public Player(UnitManager givenUnitManager)
    {
        this.unitManager = givenUnitManager;
    }

    internal void InstantiateIntialUnits(Tuple<int, int> initialPosition)
    {
        unitManager.InstantiateIntialUnits(initialPosition);
    }

    internal void SetGrid(HexGrid hexGrid)
    {
        unitManager.Grid = hexGrid;
    }
}

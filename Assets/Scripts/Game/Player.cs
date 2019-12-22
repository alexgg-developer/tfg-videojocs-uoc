using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TechnologyInfo;

public class Player
{
    protected UnitManager unitManager;
    protected CityManager cityManager;
    protected TechnologyManager technologyManager;
    //uint shields = 0;
    protected uint shields = 200;
    public uint Shields { get { return shields; } set { shields = value; } }
    protected int score = 0;
    public int Score { get { return score; } set { score = value; } }
    public int PlayerID { get { return unitManager.PlayerID; } set { unitManager.PlayerID = value; } }
    protected bool isDead = false;
    public bool IsDead { get { return isDead; } set { isDead = value; } }


    public Player(UnitManager aUnitManager, CityManager aCityManager, TechnologyManager aTechnologyManager)
    {
        this.unitManager = aUnitManager;
        this.cityManager = aCityManager;
        this.technologyManager = aTechnologyManager;
    }

    internal List<GameObject> GetUnits()
    {
        return this.unitManager.Units;
    }

    internal void InstantiateInitialUnits(Tuple<int, int> initialPosition)
    {
        unitManager.InstantiateInitialUnits(initialPosition);
    }

    public void SetGrid(HexGrid hexGrid)
    {
        unitManager.Grid = hexGrid;
        cityManager.Grid = hexGrid;
    }

    public void AddCity(HexCoordinates hexCoordinates)
    {
        cityManager.AddCity(hexCoordinates);
    }

    public void ResetUnitMovement()
    {
        unitManager.ResetUnitMovement();
    }

    public void RemoveUnit(Unit unit)
    {
        unitManager.RemoveUnit(unit);
    }

    public uint CalculateShieldsPerTurn()
    {
        return cityManager.CalculateShieldsPerTurn();
    }

    public void ProduceShields()
    {
        Shields += CalculateShieldsPerTurn();
    }

    internal Unit AddUnit(UnitStats.UnitType unitType, HexCell cell)
    {
        return unitManager.AddUnitInCell(cell, unitType);
    }

    internal City RemoveCity(int cityID)
    {
        return cityManager.RemoveCity(cityID);
    }

    internal void TransferCity(City city)
    {
        cityManager.TransferCity(city);
    }

    internal void AddTechnology(TechnologyType technology)
    {
        technologyManager.Add(technology);
    }

    internal bool HasTechnology(TechnologyType technologyType)
    {
        return technologyManager.HasTechnology(technologyType);
    }

    public bool CheckDeath()
    {
        isDead = cityManager.GetNumberOfCities() == 0;
        isDead = isDead && !unitManager.HasSettler();

        return isDead;
    }

    internal City FindNearestCity(HexCoordinates resourceCoordinates)
    {
        var city = cityManager.FindNearestCity(resourceCoordinates);
        return city;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject[] cityPrefabs;
    [SerializeField]
    GameObject[] cityPrefabsRed;
    [SerializeField]
    HexGrid grid;
    
    GameObject[] currentCityPrefabs;

    public HexGrid Grid { get { return grid; } set { grid = value; } }
    int playerID = 0;
    public int PlayerID { get { return playerID; } set { playerID = value; } }

    //List<GameObject> cityInstances = new List<GameObject>();
    List<City> cities = new List<City>();

    public void Start()
    {
        switch (playerID) {
            case 0:
                currentCityPrefabs = cityPrefabs;
                break;
            case 1:
                currentCityPrefabs = cityPrefabsRed;
                break;
        }
    }
    public void AddCity(int index)
    {
        HexCell cell = grid.GetCell(index);
        City city = AddCityInCell(cell);
        city.HasAccesToWater = cell.HasWaterInDaNeighborhood();
    }

    public void AddCity(HexCoordinates hexCoordinates)
    {
        HexCell cell = grid.GetCell(hexCoordinates.X, hexCoordinates.Z);
        City city = AddCityInCell(cell);
        city.HasAccesToWater = cell.HasWaterInDaNeighborhood();
    }

    private City AddCityInCell(HexCell cell)
    {
        GameObject cityInstance = Instantiate(currentCityPrefabs[0], cell.transform);
        City cityComponent = cityInstance.GetComponent<City>();
        cityComponent.Name = "City " + (cities.Count + 1);
        cityComponent.PlayerID = playerID;
        float offsetY = cityInstance.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y * cityInstance.transform.localScale.y * 0.5f;
        cityInstance.transform.Translate(new Vector3(0f, offsetY, 0f));
        cities.Add(cityComponent);

        return cityComponent;
    }

    public uint CalculateShieldsPerTurn()
    {
        uint shields = 0;
        foreach(City city in cities) {
            shields += city.Population * City.SHIELDS_PER_POPULATION;
        }

        return shields;
    }

    internal City RemoveCity(int cityID)
    {
        for(int i = 0; i < cities.Count; ++i) {
            if(cities[i].ID == cityID) {
                City city = cities[i];
                cities.RemoveAt(i);
                return city;
            }
        }
        return null;
    }

    internal void TransferCity(City city)
    {
        cities.Add(city);
    }

    internal int GetNumberOfCities()
    {
        return cities.Count;
    }

    internal City FindNearestCity(HexCoordinates resourceCoordinates)
    {
        City nearestCity = null;
        int nearestDistance = Int32.MaxValue;

        foreach(City city in cities) {
            int distance = HexCoordinates.distance(resourceCoordinates, city.transform.parent.GetComponent<HexCell>().coordinates);
            //if(distance < nearestDistance) {
            if (distance == 1) {
                nearestDistance = distance;
                nearestCity = city;
                break;
            }
        }

        return nearestCity;
    }

    internal List<City> GetCities()
    {
        return cities;
    }
}

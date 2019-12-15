using System;
using UnityEngine;

public class CityController : MonoBehaviour
{

#pragma warning disable 0649
    [SerializeField]
    HexGrid hexGrid;
    [SerializeField]
    CityPanel cityInfoPanel;
#pragma warning restore 0649

    Logic logic;
    int currentPlayerID = 0;
    HexCell selectedCell = null;


    // Start is called before the first frame update
    void Start()
    {
        logic = this.gameObject.GetComponent<Logic>();
    }


    public void Select()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null /*&& cell.gameObject.transform.childCount > 0*/) {
                City city = cell.gameObject.GetComponentInChildren<City>();
                if (city != null) {
                    if (city.PlayerID == currentPlayerID) {
                        Select(cell, city);
                    }                    
                }
            }
        }
    }

    public void Select(HexCell cell, City city)
    {
        Unselect();
        cell.EnableHighlight(city.PlayerID);
        selectedCell = cell;

        cityInfoPanel.SelectedCity = city;
        cityInfoPanel.gameObject.SetActive(true);
    }

    public void OnChangeOfPlayer(int newPlayerID)
    {
        currentPlayerID = newPlayerID;
        Unselect();
    }

    internal void Unselect()
    {
        if (selectedCell != null) {
            selectedCell.DisableHighlight();
            selectedCell = null;
        }
        if (cityInfoPanel != null) {
            cityInfoPanel.gameObject.SetActive(false);
        }
    }
}

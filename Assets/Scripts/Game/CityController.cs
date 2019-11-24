using System;
using UnityEngine;

public class CityController : MonoBehaviour
{
    [SerializeField]
    HexGrid hexGrid;
    [SerializeField]
    CityPanel cityInfoPanel;

    Logic logic;
    //City selectedCity = null;
    int currentPlayerID = 0;


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
                        cityInfoPanel.SelectedCity = city;
                        cityInfoPanel.gameObject.SetActive(true);
                    }                    
                }
            }
        }
    }
    
    public void OnChangeOfPlayer(int newPlayerID)
    {
        currentPlayerID = newPlayerID;
        Unselect();
    }

    internal void Unselect()
    {
        //selectedCity = null;
        if (cityInfoPanel != null) {
            cityInfoPanel.gameObject.SetActive(false);
        }
    }
}

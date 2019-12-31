using System;
using UnityEngine;
using static TechnologyInfo;

public class ResourceController : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    HexGrid hexGrid;
    [SerializeField]
    InMapResourcePanel inMapResourcePanel;
    [SerializeField]
    InfoUserCanvas infoUserCanvas;
#pragma warning restore 0649

    City nearestCity = null;
    public City NearestCity
    {
        get
        {
            return nearestCity;
        }
        set
        {
            nearestCity = value;
        }
    }

    const uint COST_COLLECT = 10;

    Logic logic;
    //int currentPlayerID = 0;
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
            if (cell != null) {
                if (cell.HasResource) {
                    Select(cell);
                }
            }
        }
    }

    public void Select(HexCell cell)
    {
        Unselect();
        cell.EnableHighlight(logic.CurrentPlayer);
        selectedCell = cell;

        nearestCity = logic.FindNearestCity(selectedCell.coordinates);
        inMapResourcePanel.HasMoney = logic.IsThereEnoughShields(COST_COLLECT);
        if(nearestCity == null) {
            inMapResourcePanel.IsCityNear = false;
        }
        else {
            inMapResourcePanel.IsCityNear = true;
            inMapResourcePanel.NearestCity = nearestCity.Name;
        }
        if (cell.ResourceKind == MapResource.ResourceKind.CORN) {
            inMapResourcePanel.HasTechnology = logic.HasTechnology(TechnologyType.AGRICULTURE);
        }
        else if (cell.ResourceKind == MapResource.ResourceKind.FISH) {
            inMapResourcePanel.HasTechnology = logic.HasTechnology(TechnologyType.NAVIGATION);
        }
        //inMapResourcePanel.gameObject.SetActive(true);
        infoUserCanvas.OpenInMapResourcePanel();
    }
    
    internal void Unselect()
    {
        if (selectedCell != null) {
            selectedCell.DisableHighlight();
            selectedCell = null;
        }

        if (inMapResourcePanel != null) {
            inMapResourcePanel.gameObject.SetActive(false);
        }
    }

    internal void CollectResource()
    {
        logic.OnScoreEvent(ScoreManager.TypesScore.RESOURCE, logic.CurrentPlayer);
        selectedCell.SetResource(MapResource.ResourceKind.NONE);
        selectedCell.DisableHighlight();
        nearestCity.GrowCity();
        selectedCell = null;
        nearestCity = null;
        inMapResourcePanel.gameObject.SetActive(false);
    }
}

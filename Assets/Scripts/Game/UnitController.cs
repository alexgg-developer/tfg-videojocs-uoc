﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitController : MonoBehaviour
{
    const float DEFENSE_BONUS_CITY = 0.5f; //50% more
    [System.Serializable] public class UnityScoreEvent : UnityEvent<ScoreManager.TypesScore, int> { }
    [SerializeField]
    UnityScoreEvent scoreEvent;
    public HexGrid hexGrid;
    Logic logic;
    GameObject selectedUnitGO = null;
    int currentPlayerID = 0;
    // Start is called before the first frame update
    void Start()
    {
        logic = this.gameObject.GetComponent<Logic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedUnitGO != null) {
            if (Input.GetKeyUp(KeyCode.B)) {
                var unit = selectedUnitGO.GetComponent<Unit>();
                if (unit.Type == UnitStats.UnitType.SETTLER) {
                    if (logic.BuildCity(selectedUnitGO.GetComponentInParent<HexCell>().coordinates)) {
                        logic.RemoveUnit(unit);
                        Destroy(selectedUnitGO);
                        selectedUnitGO = null;
                    }
                }
            }
        }

        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetMouseButtonUp(0)) {
                Select();
            }
            else if (Input.GetMouseButtonUp(1)) {
                Move();
            }
        }
        //GameObject child = originalGameObject.transform.GetChild(0).gameObject;
    }

    private void Move()
    {
        if (selectedUnitGO == null) return;
        Unit selectedUnit = selectedUnitGO.GetComponent<Unit>();

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null) {
                int distance = HexCoordinates.distance(cell.coordinates, selectedUnitGO.transform.parent.GetComponent<HexCell>().coordinates);
                if (distance == 1) {
                    if (selectedUnit.MovementLeft < 1.0f) return;

                    if (cell != selectedUnitGO.transform.parent) {
                        if (cell != null && cell.gameObject.transform.childCount > 0) {
                            City goalCity = cell.gameObject.transform.GetComponentInChildren<City>();
                            Unit goalUnit = cell.gameObject.transform.GetComponentInChildren<Unit>();
                            bool isDefenderInCity = goalCity != null;

                            if (goalUnit == null || goalUnit.PlayerID == currentPlayerID || selectedUnit.HasAttacked) return;

                            if (Fight(selectedUnitGO.GetComponent<Unit>(), goalUnit, isDefenderInCity)) {
                                MoveUnit(selectedUnitGO, cell);
                                scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT, selectedUnit.PlayerID);
                                logic.RemoveUnit(goalUnit);
                                Destroy(goalUnit.gameObject);
                            }
                            else {
                                scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT, goalUnit.PlayerID);
                                logic.RemoveUnit(selectedUnit);
                                Destroy(selectedUnitGO);
                                selectedUnitGO = null;
                            }
                            return;
                        }
                        MoveUnit(selectedUnitGO, cell);
                    }
                    else {
                        Debug.Log("same cell");
                    }
                }
                else if (distance == 2 && (selectedUnit.Type == UnitStats.UnitType.ARCHER || selectedUnit.Type == UnitStats.UnitType.CATAPULT)) {
                    //GameObject goalUnitGO = cell.gameObject.transform.GetChild(0).gameObject;
                    City goalCity = cell.gameObject.transform.GetComponentInChildren<City>();
                    Unit goalUnit = cell.gameObject.transform.GetComponentInChildren<Unit>();
                    bool isDefenderInCity = goalCity != null;

                    if (goalUnit == null || goalUnit.PlayerID == currentPlayerID || selectedUnit.HasAttacked) return;

                    bool isDead = false;
                    if (selectedUnit.Type == UnitStats.UnitType.CATAPULT && isDefenderInCity) {
                        isDead = DistanceFight(selectedUnit, goalUnit, isDefenderInCity);
                    }
                    else {
                        isDead = DistanceFight(selectedUnit, goalUnit, isDefenderInCity);
                    }
                    if(isDead) {
                        scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT, selectedUnit.PlayerID);
                        logic.RemoveUnit(goalUnit);
                        Destroy(goalUnit.gameObject);
                    }
                }
            }
        }
    }

    void MoveUnit(GameObject unitToMove, HexCell cellToMoveTo)
    {
        unitToMove.transform.SetParent(cellToMoveTo.transform);
        float offsetY = unitToMove.GetComponent<MeshFilter>().mesh.bounds.size.y * unitToMove.transform.localScale.y * 0.5f;
        selectedUnitGO.transform.localPosition = new Vector3(0f, offsetY, 0f);
        --unitToMove.GetComponent<Unit>().MovementLeft;
    } 

    void Select()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null && cell.gameObject.transform.childCount > 0) {
                int childs = cell.gameObject.transform.childCount;
                for (int i = 0; i < childs; ++i) {
                    GameObject child = cell.gameObject.transform.GetChild(i).gameObject;
                    Unit unit;
                    if (child.TryGetComponent<Unit>(out unit)) {
                        if (unit.PlayerID == currentPlayerID) {
                            child.transform.Translate(3f, 0f, 0f);
                            selectedUnitGO = child;
                        }
                    }
                }
            }
        }
    }

    bool Fight(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity)
    {
        bool hasWon = false;
        selectedUnit.HasAttacked = true;

        while (selectedUnit.CurrentHealth > 0 && goalUnit.CurrentHealth > 0) {
            Attack(selectedUnit, goalUnit, isDefenderInCity, false);
        }

        if (selectedUnit.CurrentHealth == 0) {
            Debug.Log("defender winner");
        }
        else {
            Debug.Log("attacker winner");
            hasWon = true;
        }
        Debug.Log("selectedUnit.CurrentHealth:" + selectedUnit.CurrentHealth);
        Debug.Log("goalUnit.CurrentHealth:" + goalUnit.CurrentHealth);

        return hasWon;
    }

    bool DistanceFight(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity)
    {
        selectedUnit.HasAttacked = true;
        //3 attacks
        bool goalUnitIsDead = goalUnit.CurrentHealth <= 0.0f;
        for (int i = 0; i < 300 && !goalUnitIsDead; ++i) {
            Attack(selectedUnit, goalUnit, isDefenderInCity, true);
            goalUnitIsDead = goalUnit.CurrentHealth <= 0.0f;
        }

        return goalUnitIsDead;
    }

    void Attack(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity, bool distanceAttack)
    {
        float bonusCity = isDefenderInCity ? goalUnit.Defense * 0.5f : 0.0f;
        float totalAttack = selectedUnit.Attack + (goalUnit.Defense + bonusCity);
        float probabilityVictorySelected = (float)selectedUnit.Attack / (float)totalAttack;
        float attackResult = UnityEngine.Random.Range(0f, 1f);
        if (attackResult <= probabilityVictorySelected) {
            goalUnit.CurrentHealth--;
        }
        else if(!distanceAttack) {
            selectedUnit.CurrentHealth--;
        }
        /*Debug.Log("attackResult::" + attackResult);
        Debug.Log("selectedUnit.CurrentHealth::" + selectedUnit.CurrentHealth);
        Debug.Log("goalUnit.CurrentHealth::" + goalUnit.CurrentHealth);*/
    }

    public void OnChangeOfPlayer(int newPlayerID)
    {
        currentPlayerID = newPlayerID;
        selectedUnitGO = null;
    }
}

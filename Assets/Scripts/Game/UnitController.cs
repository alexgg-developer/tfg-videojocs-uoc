using UnityEngine;
using UnityEngine.EventSystems;

public class UnitController : MonoBehaviour
{
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
        if (selectedUnit.MovementLeft < 1.0f) return;

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null) {
                int distance = HexCoordinates.distance(cell.coordinates, selectedUnitGO.transform.parent.GetComponent<HexCell>().coordinates);
                if (distance == 1) {
                    if (cell != selectedUnitGO.transform.parent) {
                        if (cell != null && cell.gameObject.transform.childCount > 0) {
                            GameObject child = cell.gameObject.transform.GetChild(0).gameObject;
                            Unit goalUnit = child.GetComponent<Unit>();
                            if (goalUnit != null) {
                                if (goalUnit.PlayerID != currentPlayerID) {
                                    if(Fight(selectedUnitGO.GetComponent<Unit>(), goalUnit)) {
                                        MoveUnit(selectedUnitGO, cell);
                                        logic.RemoveUnit(goalUnit);
                                    }
                                    else {
                                        logic.RemoveUnit(selectedUnit);
                                        Destroy(selectedUnitGO);
                                        selectedUnitGO = null;
                                    }
                                }
                                return;
                            }
                        }
                        MoveUnit(selectedUnitGO, cell);
                    }
                    else {
                        Debug.Log("same cell");
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

    bool Fight(Unit selectedUnit, Unit goalUnit)
    {
        bool hasWon = false;
        int totalAttack = selectedUnit.Attack + goalUnit.Attack;
        float probabilityVictorySelected = (float)selectedUnit.Attack / (float)totalAttack;
        while (selectedUnit.CurrentHealth > 0 && goalUnit.CurrentHealth > 0) {
            float attackResult = UnityEngine.Random.Range(0f, 1f);
            if (attackResult <= probabilityVictorySelected) {
                goalUnit.CurrentHealth--;
            }
            else {
                selectedUnit.CurrentHealth--;
            }
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

    public void OnChangeOfPlayer(int newPlayerID)
    {
        currentPlayerID = newPlayerID;
        selectedUnitGO = null;
    }
}

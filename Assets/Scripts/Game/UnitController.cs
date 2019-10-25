using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitController : MonoBehaviour
{
    public HexGrid hexGrid;

    GameObject selectedUnit = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
        if (selectedUnit == null) return;

        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null) {
                if (cell != selectedUnit.transform.parent) {
                    //selectedUnit.transform = cell.transform;
                    if (cell != null && cell.gameObject.transform.childCount > 0) {
                        GameObject child = cell.gameObject.transform.GetChild(0).gameObject;
                        Unit goalUnit = child.GetComponent<Unit>();
                        Fight(selectedUnit.GetComponent<Unit>(), goalUnit);
                        return;
                    }
                    selectedUnit.transform.SetParent(cell.transform);
                    float offsetY = selectedUnit.GetComponent<MeshFilter>().mesh.bounds.size.y * selectedUnit.transform.localScale.y * 0.5f;
                    selectedUnit.transform.localPosition = new Vector3(0f, offsetY, 0f);
                }
                else {
                    Debug.Log("same cell");
                }
            }
        }
    }

    void Select()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null && cell.gameObject.transform.childCount > 0) {
                GameObject child = cell.gameObject.transform.GetChild(0).gameObject;
                if (child != null) {
                    child.transform.Translate(3f, 0f, 0f);
                    selectedUnit = child;
                }
            }
        }
    }

    void Fight(Unit selectedUnit, Unit goalUnit)
    {
        int totalAttack = selectedUnit.Attack + goalUnit.Attack;
        float probabilityVictorySelected = (float)selectedUnit.Attack / (float)totalAttack;
        Debug.Log("selectedUnit.CurrentHealth:" + selectedUnit.CurrentHealth);
        Debug.Log("goalUnit.CurrentHealth:" + goalUnit.CurrentHealth);
        while (selectedUnit.CurrentHealth > 0 && goalUnit.CurrentHealth > 0) {
            float attackResult = UnityEngine.Random.Range(0f, 1f);
            if(attackResult <= probabilityVictorySelected) {
                goalUnit.CurrentHealth--;
            }
            else {
                selectedUnit.CurrentHealth--;
            }
        }
        if(selectedUnit.CurrentHealth == 0) {
            Debug.Log("defender winner");
        }
        else {
            Debug.Log("attacker winner");
        }
        Debug.Log(" end selectedUnit.CurrentHealth:" + selectedUnit.CurrentHealth);
        Debug.Log("end goalUnit.CurrentHealth:" + goalUnit.CurrentHealth);
        selectedUnit.CurrentHealth = 10 ;
        goalUnit.CurrentHealth = 10;
    }
}

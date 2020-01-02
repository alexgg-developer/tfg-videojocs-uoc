using System;
using UnityEngine;
using UnityEngine.Events;

public class UnitController : MonoBehaviour
{
    const float DEFENSE_BONUS_CITY = 0.5f; //50% more

#pragma warning disable 0649
    [System.Serializable]
    public class UnityScoreEvent : UnityEvent<ScoreManager.TypesScore, int> { }
    [SerializeField]
    UnityScoreEvent scoreEvent;
    [SerializeField]
    HexGrid hexGrid;
    [SerializeField]
    InfoUserCanvas infoUserCanvas;
#pragma warning restore 0649

    Logic logic;
    AnimationController animationController;
    int currentPlayerID = 0;

    GameObject selectedUnitGO = null;
    HexCell selectedCell = null;
    public UnityScoreEvent ScoreEvent
    {
        get
        {
            return scoreEvent;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        animationController = logic.GetComponent<AnimationController>();
    }

    public void Move()
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
                        if (selectedUnit.Type == UnitStats.UnitType.SHIP) { MoveShip(cell, selectedUnit); return; }
                        else if (cell.IsUnderwater) return;

                        int childCount = cell.gameObject.transform.childCount;
                        if (cell.HasResource) --childCount;
                        if (childCount > 0) {
                            City goalCity = cell.gameObject.transform.GetComponentInChildren<City>();
                            Unit goalUnit = cell.gameObject.transform.GetComponentInChildren<Unit>();
                            bool thereIsCity = goalCity != null;
                            if (thereIsCity && goalUnit == null) {
                                if (goalCity.PlayerID == currentPlayerID) {
                                    MoveUnit(selectedUnitGO, cell);
                                    return;
                                }
                                else if (goalCity.PlayerID != currentPlayerID) {
                                    //MoveUnit(selectedUnitGO, cell);
                                    //ConquerCity(conquererPlayer, conqueredPlayer);
                                    ConquerCity(currentPlayerID, goalCity.PlayerID, goalCity.ID);
                                    MoveUnit(selectedUnitGO, cell);
                                    //Debug.Log("ConquerCity");
                                    return;
                                }
                            }

                            if (goalUnit == null || goalUnit.PlayerID == currentPlayerID) return;
                            if (selectedUnit.HasAttacked || selectedUnit.Type == UnitStats.UnitType.CATAPULT || selectedUnit.Type == UnitStats.UnitType.SETTLER) return;

                            FightLogic(selectedUnitGO.GetComponent<Unit>(), goalUnit, thereIsCity, cell, goalCity);
                            return;
                        }
                        else {
                            MoveUnit(selectedUnitGO, cell);
                        }
                    }
                    else {
                        Debug.Log("same cell");
                    }
                }
                else if (distance == 2 &&
                    (selectedUnit.Type == UnitStats.UnitType.ARCHER || selectedUnit.Type == UnitStats.UnitType.CATAPULT || selectedUnit.Type == UnitStats.UnitType.SHIP)) {
                    //GameObject goalUnitGO = cell.gameObject.transform.GetChild(0).gameObject;
                    City goalCity = cell.gameObject.transform.GetComponentInChildren<City>();
                    Unit goalUnit = cell.gameObject.transform.GetComponentInChildren<Unit>();
                    bool isDefenderInCity = goalCity != null;

                    if (goalUnit == null || goalUnit.PlayerID == currentPlayerID || selectedUnit.HasAttacked) return;

                    Action winAction = () =>
                    {
                        scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT, selectedUnit.PlayerID);
                        logic.RemoveUnit(goalUnit);
                    };
                    DistanceFight(selectedUnit, goalUnit, isDefenderInCity, winAction);
                }
            }
        }
    }

    private void MoveShip(HexCell cell, Unit selectedUnit)
    {
        int childCount = cell.gameObject.transform.childCount;
        if (cell.HasResource) --childCount;
        if (childCount > 0) {
            City goalCity = cell.gameObject.transform.GetComponentInChildren<City>();
            Unit goalUnit = cell.gameObject.transform.GetComponentInChildren<Unit>();
            bool thereIsCity = goalCity != null;
            if (thereIsCity && goalUnit == null) {
                if (goalCity.PlayerID == currentPlayerID) {
                    MoveUnit(selectedUnitGO, cell);
                    return;
                }
            }

            if (goalUnit == null || goalUnit.PlayerID == currentPlayerID) return;
            if (selectedUnit.HasAttacked || selectedUnit.Type == UnitStats.UnitType.CATAPULT || selectedUnit.Type == UnitStats.UnitType.SETTLER) return;
            Action winAction = () =>
            {
                scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT, selectedUnit.PlayerID);
                logic.RemoveUnit(goalUnit);
            };
            DistanceFight(selectedUnit, goalUnit, thereIsCity, winAction);
        }
        else {
            if (cell.IsUnderwater) {
                MoveUnit(selectedUnitGO, cell);
            }
        }
    }

    private void ConquerCity(int conquererPlayerID, int defeatedPlayerID, int cityID)
    {
        logic.TransferCity(defeatedPlayerID, conquererPlayerID, cityID);
        scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT_CITY, conquererPlayerID);
    }

    void MoveUnit(GameObject unitToMove, HexCell cellToMoveTo)
    {
        float offsetY = unitToMove.GetComponent<MeshFilter>().mesh.bounds.size.y * unitToMove.transform.localScale.y * 0.5f;
        Vector3 newPosition = new Vector3(cellToMoveTo.transform.position.x, cellToMoveTo.transform.position.y + offsetY, cellToMoveTo.transform.position.z);
        Vector3 movement = newPosition - unitToMove.transform.position;
        MoveBy moveUnitAnimationBy = new MoveBy(movement, 0.25f, unitToMove);


        Caller caller = new Caller();
        caller.ActionFunction = () => MoveUnitCallback(unitToMove, cellToMoveTo);

        Sequencer sequence = new Sequencer();
        sequence.Add(moveUnitAnimationBy);
        sequence.Add(caller);

        animationController.Add(sequence);

    }

    void MoveUnitCallback(GameObject unitToMove, HexCell cellToMoveTo)
    {

        unitToMove.transform.SetParent(cellToMoveTo.transform);
        float offsetY = unitToMove.GetComponent<MeshFilter>().mesh.bounds.size.y * unitToMove.transform.localScale.y * 0.5f;
        unitToMove.transform.localPosition = new Vector3(0f, offsetY, 0f);
        var unit = unitToMove.GetComponent<Unit>();
        --unit.MovementLeft;
        unit.OnNewPosition();
        SwitchSelectedCell(cellToMoveTo, unit.PlayerID);
        infoUserCanvas.UpdateUnitPanel();
    }

    public void Select()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            HexCell cell = hexGrid.GetCell(hit.point);
            if (cell != null && cell.gameObject.transform.childCount > 0) {
                int childs = cell.gameObject.transform.childCount;
                for (int i = 0; i < childs; ++i) {
                    GameObject child = cell.gameObject.transform.GetChild(i).gameObject;
                    Unit unit = null;
                    if (child.TryGetComponent<Unit>(out unit)) {
                        if (unit.PlayerID == currentPlayerID) {
                            Select(cell, unit);
                        }
                    }
                }
            }
        }
    }

    public void Select(HexCell cell, Unit unit)
    {
        Unselect();
        cell.EnableHighlight(unit.PlayerID);
        selectedUnitGO = unit.gameObject;
        selectedCell = cell;
        if (infoUserCanvas != null) {
            infoUserCanvas.OpenUnitPanel(unit);
        }
    }

    public void Unselect()
    {
        if (selectedCell != null) {
            selectedCell.DisableHighlight();
            selectedCell = null;
        }
        selectedUnitGO = null;
        if (infoUserCanvas != null) {
            infoUserCanvas.CloseUnitPanel();
        }
    }

    public void SwitchSelectedCell(HexCell cell, int playerID)
    {
        if (selectedCell != null) {
            selectedCell.DisableHighlight();
            selectedCell = cell;
            cell.EnableHighlight(playerID);
        }
    }


    public void FightLogic(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity, HexCell goalCell, City goalCity = null)
    {
        if (goalUnit != null) {
            Fight(selectedUnit, goalUnit, isDefenderInCity, goalCell, goalCity);
        }
    }



    void Attack(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity, bool distanceAttack)
    {
        float bonusCity = isDefenderInCity ? goalUnit.Defense * 0.5f : 0.0f;
        bonusCity = selectedUnit.Type == UnitStats.UnitType.CATAPULT ? 0.0f : bonusCity;
        float totalAttack = selectedUnit.Attack + (goalUnit.Defense + bonusCity);
        float probabilityVictorySelected = (float)selectedUnit.Attack / (float)totalAttack;
        float attackResult = UnityEngine.Random.Range(0f, 1f);
        if (attackResult <= probabilityVictorySelected) {
            goalUnit.CurrentHealth--;
        }
        else if (!distanceAttack) {
            selectedUnit.CurrentHealth--;
        }

    }

    public void DistanceAttackAnimation(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity, Sequencer sequencer,
        ref int goalUnitCurrentHealth)
    {
        float bonusCity = isDefenderInCity ? goalUnit.Defense * 0.5f : 0.0f;
        bonusCity = selectedUnit.Type == UnitStats.UnitType.CATAPULT ? 0.0f : bonusCity;
        float totalAttack = selectedUnit.Attack + (goalUnit.Defense + bonusCity);
        float probabilityVictorySelected = (float)selectedUnit.Attack / (float)totalAttack;

        Unit unitToMove = selectedUnit;
        Unit unitToHurt = goalUnit;
        goalUnitCurrentHealth--;
        float offsetY = unitToMove.gameObject.GetComponent<MeshFilter>().mesh.bounds.size.y * unitToMove.gameObject.transform.localScale.y * 0.5f;
        Vector3 newPosition = new Vector3(
            unitToHurt.gameObject.transform.position.x,
            unitToHurt.gameObject.transform.position.y + offsetY,
            unitToHurt.gameObject.transform.position.z);
        Vector3 movement = newPosition - unitToMove.gameObject.transform.position;
        movement = movement * 0.1f;
        MoveBy unitMovementAnimation = new MoveBy(movement, 0.1f, unitToMove.gameObject);
        sequencer.Add(unitMovementAnimation);
        Caller togglePunchCaller = new Caller();
        togglePunchCaller.ActionFunction = () => unitToHurt.ToggleWound(true);
        sequencer.Add(togglePunchCaller);

        MoveBy reverseUnitMovementAnimation = new MoveBy(-movement, 0.15f, unitToMove.gameObject);
        sequencer.Add(reverseUnitMovementAnimation);
        Caller reverseTogglePunchCaller = new Caller();
        reverseTogglePunchCaller.ActionFunction = () => unitToHurt.ToggleWound(false);
        sequencer.Add(reverseTogglePunchCaller);

        Caller caller = new Caller();
        caller.ActionFunction = () => unitToHurt.CurrentHealth--;
        sequencer.Add(caller);
    }

    public void AttackAnimation(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity, Sequencer sequencer,
        ref int selectedUnitCurrentHealth, ref int goalUnitCurrentHealth, bool distanceAttack)
    {
        float bonusCity = isDefenderInCity ? goalUnit.Defense * 0.5f : 0.0f;
        bonusCity = selectedUnit.Type == UnitStats.UnitType.CATAPULT ? 0.0f : bonusCity;
        float totalAttack = selectedUnit.Attack + (goalUnit.Defense + bonusCity);
        float probabilityVictorySelected = (float)selectedUnit.Attack / (float)totalAttack;
        float attackResult = UnityEngine.Random.Range(0f, 1f);

        Unit unitToMove = null;
        Unit unitToHurt = null;
        if (attackResult <= probabilityVictorySelected) {
            goalUnitCurrentHealth--;
            unitToMove = selectedUnit;
            unitToHurt = goalUnit;
        }
        else if (!distanceAttack) {
            selectedUnitCurrentHealth--;
            unitToMove = goalUnit;
            unitToHurt = selectedUnit;
        }

        float offsetY = unitToMove.gameObject.GetComponent<MeshFilter>().mesh.bounds.size.y * unitToMove.gameObject.transform.localScale.y * 0.5f;
        Vector3 newPosition = new Vector3(
            unitToHurt.gameObject.transform.position.x,
            unitToHurt.gameObject.transform.position.y + offsetY,
            unitToHurt.gameObject.transform.position.z);
        Vector3 movement = newPosition - unitToMove.gameObject.transform.position;
        MoveBy unitMovementAnimation = new MoveBy(movement, 0.15f, unitToMove.gameObject);
        sequencer.Add(unitMovementAnimation);
        Caller togglePunchCaller = new Caller();
        togglePunchCaller.ActionFunction = () => unitToHurt.ToggleWound(true);
        sequencer.Add(togglePunchCaller);
        MoveBy reverseUnitMovementAnimation = new MoveBy(-movement, 0.15f, unitToMove.gameObject);
        sequencer.Add(reverseUnitMovementAnimation);
        Caller reverseTogglePunchCaller = new Caller();
        reverseTogglePunchCaller.ActionFunction = () => unitToHurt.ToggleWound(false);
        sequencer.Add(reverseTogglePunchCaller);

        Caller caller = new Caller();
        caller.ActionFunction = () => unitToHurt.CurrentHealth--;
        sequencer.Add(caller);
    }

    public void HurtCallback(Unit unit)
    {
        unit.CurrentHealth--;
    }

    public void Fight(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity, HexCell goalCell, City goalCity = null)
    {
        selectedUnit.HasAttacked = true;
        selectedUnit.MovementLeft = 0.0f;

        var selectedUnitCurrentHealth = selectedUnit.CurrentHealth;
        var goalUnitCurrentHealth = goalUnit.CurrentHealth;

        Sequencer sequencer = new Sequencer();
        while (selectedUnitCurrentHealth > 0 && goalUnitCurrentHealth > 0) {
            AttackAnimation(selectedUnit, goalUnit, isDefenderInCity, sequencer, ref selectedUnitCurrentHealth, ref goalUnitCurrentHealth, false);
        }

        Caller caller = new Caller();

        if (selectedUnitCurrentHealth == 0) {
            caller.ActionFunction = () =>
            {
                Debug.Log("defender winner");
                scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT, goalUnit.PlayerID);
                logic.RemoveUnit(selectedUnit);
                Unselect();
            };
        }
        else {
            caller.ActionFunction = () =>
            {
                Debug.Log("attacker winner");
                MoveUnit(selectedUnit.gameObject, goalCell);
                if (goalUnit != null) {
                    scoreEvent.Invoke(ScoreManager.TypesScore.FIGHT, selectedUnit.PlayerID);
                    logic.RemoveUnit(goalUnit);
                }
                if (isDefenderInCity) {
                    ConquerCity(currentPlayerID, goalCity.PlayerID, goalCity.ID);
                }
            };
        }
        sequencer.Add(caller);

        animationController.Add(sequencer);
    }

    public void DistanceFight(Unit selectedUnit, Unit goalUnit, bool isDefenderInCity, Action winAction)
    {
        selectedUnit.HasAttacked = true;
        //3 attacks
        Sequencer sequencer = new Sequencer();
        int goalUnitCurrentHealth = goalUnit.CurrentHealth;
        bool goalUnitIsDead = goalUnitCurrentHealth <= 0.0f;
        for (int i = 0; i < 3 && !goalUnitIsDead; ++i) {
            DistanceAttackAnimation(selectedUnit, goalUnit, isDefenderInCity, sequencer, ref goalUnitCurrentHealth);
            goalUnitIsDead = goalUnitCurrentHealth <= 0.0f;
        }
        if(goalUnitIsDead) {
            Caller caller = new Caller();
            caller.ActionFunction = winAction;
            sequencer.Add(caller);
        }
        animationController.Add(sequencer);
    }

    public void OnChangeOfPlayer(int newPlayerID)
    {
        currentPlayerID = newPlayerID;
        Unselect();
    }

    public void SettlerCommand()
    {
        if (selectedUnitGO == null) return;
        var unit = selectedUnitGO.GetComponent<Unit>();
        if (unit.Type == UnitStats.UnitType.SETTLER) {
            var cell = selectedUnitGO.GetComponentInParent<HexCell>();
            if (logic.BuildCity(cell.coordinates)) {
                logic.RemoveUnit(unit);
                Unselect();
                cell.SetResource(MapResource.ResourceKind.NONE);
            }
        }
    }

}

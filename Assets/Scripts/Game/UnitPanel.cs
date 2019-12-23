using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static BuildingInfo;
using static UnitStats;

public class UnitPanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject descriptionSelectedPanel;
    [SerializeField]
    Text descriptionSelectedText;
    [SerializeField]
    Text attackText;
    [SerializeField]
    Text defenseText;
    [SerializeField]
    Text lifeText;
    [SerializeField]
    Text movementText;
    [SerializeField]
    Button[] buyingButtons;
    [SerializeField]
    Text[] nameTextsButton;
    [SerializeField]
    BuildingInfo[] buildingInfos;
    [SerializeField]
    UnityEvent OnOpenEvent;
    [SerializeField]
    UnitController unitController = null;
#pragma warning restore 0649
    const uint TOTAL_BUY_BUTTONS = 3;
    //const string NOT_ENOUGH_SHIELDS = "Not enough shields.";
    const string ALREADY_HEALED = "The unit is already healed.";
    const string ALREADY_MOVED = "The unit has already moved so you can not do the action.";
    const string MOVEMENT_NEEDED = "You cannot do this action if you have already moved the unit.";

    Logic logic;
    int selectedButton = -1;
    Button actionButton = null;

    List<string> unitActionButtonName = new List<string>();
    List<string> unitActionDescription = new List<string>();

    //Dictionary<UnitType, List<UnitAction>> unitsAvailable = new List<UnitType>();
    List<UnitAction> actionsAvailable = new List<UnitAction>();

    Unit selectedUnit = null;
    public Unit SelectedUnit { get { return selectedUnit; } set { selectedUnit = value; } }

    private void Awake()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        actionButton = descriptionSelectedPanel.GetComponentInChildren<Button>();
        //ShowUnitButtons();
        //public enum UnitAction { HEAL, BUILD_CITY, SIZE };
        actionsAvailable.Add(UnitAction.HEAL);
        actionsAvailable.Add(UnitAction.BUILD_CITY);

        unitActionButtonName.Add("Heal");
        unitActionButtonName.Add("Build city");

        unitActionDescription.Add("The unit will be healed by 3 points of life. But you will spend all your movement.");
        unitActionDescription.Add("Build city in the current cell.");
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        //actionsAvailable = selectedUnit.Action;

        UpdateInfo();
        ShowUnitButtons();

        OnOpenEvent.Invoke();
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        selectedButton = -1;
        descriptionSelectedPanel.SetActive(false);
    }

    public void OnClickedBuyButton(int index)
    {
        index = (int)selectedUnit.Action;
        UnitAction unitAction = actionsAvailable[index];
        descriptionSelectedText.text = unitActionDescription[index];

        /*if (selectedButton == index) {
            MakeAction();
        }
        else {
        }*/
        selectedButton = index;
        descriptionSelectedPanel.SetActive(true);
        actionButton.gameObject.SetActive(true);
    }

    private void ShowUnitButtons()
    {
        UnitAction unitAction = UnitAction.HEAL;
        if (selectedUnit.Type == UnitType.SETTLER) {
            unitAction = UnitAction.BUILD_CITY;
        }
        Sprite sprite = Resources.Load("UI/Icons/" + unitAction.ToString(), typeof(Sprite)) as Sprite;
        buyingButtons[0].GetComponent<Image>().sprite = sprite;
        buyingButtons[0].gameObject.SetActive(true);
        nameTextsButton[0].text = unitActionButtonName[(int)unitAction];
    }

    public void MakeAction()
    {
        if (selectedButton == -1) return;

        UnitAction unitAction = actionsAvailable[selectedButton];
        //descriptionSelectedText.text = unitActionDescription[selectedButton];

        switch (unitAction) {
            case UnitAction.HEAL:
                if (selectedUnit.Movement == selectedUnit.MovementLeft) {
                    if (selectedUnit.CurrentHealth < selectedUnit.Health) {
                        selectedUnit.CurrentHealth = Math.Min(selectedUnit.CurrentHealth + Unit.HEAL_RATIO, selectedUnit.Health);
                        selectedUnit.MovementLeft = 0;
                        descriptionSelectedPanel.gameObject.SetActive(false);
                        UpdateInfo();
                    }
                    else {
                        descriptionSelectedText.text = ALREADY_HEALED;
                        //actionButton.gameObject.SetActive(false);
                    }
                }
                else {
                    descriptionSelectedText.text = ALREADY_MOVED;
                    //actionButton.gameObject.SetActive(false);
                }
                break;
            case UnitAction.BUILD_CITY:
                unitController.SettlerCommand();
                break;
        }
    }

    internal void UpdateInfo()
    {
        attackText.text = "Attack - " + selectedUnit.Attack.ToString();
        defenseText.text = "Defense - " + selectedUnit.Defense.ToString();
        lifeText.text = "Life - " + selectedUnit.CurrentHealth.ToString() + "/" + selectedUnit.Health.ToString();
        movementText.text = "Movement - " + selectedUnit.MovementLeft.ToString() + "/" + selectedUnit.Movement.ToString();

        descriptionSelectedPanel.gameObject.SetActive(false);
    }
}

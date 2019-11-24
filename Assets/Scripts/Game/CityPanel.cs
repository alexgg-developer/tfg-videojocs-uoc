﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static BuildingInfo;
using static UnitStats;

public class CityPanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject descriptionSelectedPanel;
    [SerializeField]
    Text descriptionSelectedText;
    [SerializeField]
    Text cityNameText;
    [SerializeField]
    Text shieldPerTurnText;
    [SerializeField]
    Text populationText;
    [SerializeField]
    Button showBuildingsButton;
    [SerializeField]
    Button showUnitsButton;
    [SerializeField]
    Button[] buyingButtons;
    [SerializeField]
    Text[] nameTextsButton;
    [SerializeField]
    Text[] costTextsButton;
    [SerializeField]
    UnitStats[] unitStats;
    [SerializeField]
    BuildingInfo[] buildingInfos;
#pragma warning restore 0649
    const uint TOTAL_BUY_BUTTONS = 6;
    const string NOT_ENOUGH_SHIELDS = "Not enough shields.";
    const string ALREADY_UNIT_CITY = "There is an unit already in the city.";

    Logic logic;
    bool showUnits = true;
    int selectedButton = -1;
    City selectedCity = null;
    public City SelectedCity { get { return selectedCity; } set { selectedCity = value; } }

    List<UnitType> unitsAvailable = new List<UnitType>();
    List<BuildingType> buildingsAvailable = new List<BuildingType>();
    Dictionary<UnitType, Sprite> unitSprites = new Dictionary<UnitType, Sprite>();
    Dictionary<BuildingType, Sprite> buildingSprites = new Dictionary<BuildingType, Sprite>();


    private void Awake()
    {
        unitsAvailable.Add(UnitType.WARRIOR);
        unitsAvailable.Add(UnitType.SETTLER);

        buildingsAvailable.Add(BuildingType.BARN);

        //ShowUnitButtons();
    }

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
    }

    private void OnEnable()
    {
        cityNameText.text = selectedCity.Name;
        shieldPerTurnText.text = selectedCity.ShieldsPerTurn().ToString() + " shields per turn";
        populationText.text = selectedCity.Population.ToString() + " population";
        if (showUnits) {
            ShowUnitButtons();
        }
        else {
            ShowBuildingButtons();
        }
    }

    private void OnDisable() 
    {
        Debug.Log("OnDisable");
        selectedButton = -1;
        descriptionSelectedPanel.SetActive(false);
    }

    public void OnChangeOfTurn(int newTurn)
    {
    }

    public void OnChangeOfPlayer(int newPlayerID)
    {
    }

    public void OnClickedBuyButton(int index)
    {
        if (showUnits) {
            UnitType unitType = unitsAvailable[index];
            descriptionSelectedText.text = unitStats[(int)unitType].Description;
            if (unitStats[index].Type != UnitType.SETTLER) {
                descriptionSelectedText.text += " It has " + unitStats[(int)unitType].Attack + " of attack, ";
                descriptionSelectedText.text += unitStats[(int)unitType].Defense + " of defense and ";
                descriptionSelectedText.text += unitStats[(int)unitType].Movement + " of movement.";
            }
            if (selectedButton == index) {
                if (logic.IsThereEnoughShields(unitStats[(int)unitType].ShieldCost)) {
                    if (logic.TryBuildingUnit(unitType, selectedCity)) {
                        logic.TrySpendShields(unitStats[(int)unitType].ShieldCost);
                        this.gameObject.SetActive(false);
                    }
                    else {
                        descriptionSelectedText.text = ALREADY_UNIT_CITY;
                    }
                }
                else {
                    descriptionSelectedText.text = NOT_ENOUGH_SHIELDS;
                }
            }
            else {
                selectedButton = index;
                descriptionSelectedPanel.SetActive(true);
            }
        }
        else {
            BuildingType buildingType = buildingsAvailable[index];
            descriptionSelectedText.text = buildingInfos[(int)buildingType].Description;
            if (selectedButton == index) {
                if (!selectedCity.HasBuilding(buildingType)) {
                    var shieldCost = buildingInfos[(int)buildingType].ShieldCost;
                    if (logic.TrySpendShields(shieldCost)) {
                        selectedCity.BuildBuilding(buildingType);
                        this.gameObject.SetActive(false);
                    }
                }
            }
            else {
                selectedButton = index;
                descriptionSelectedPanel.SetActive(true);
            }
        }
    }

    public void SwitchButtons()
    {
        selectedButton = -1;
        showUnits = !showUnits;
        if (showUnits) {
            ShowUnitButtons();
        }
        else {
            ShowBuildingButtons();
        }
        descriptionSelectedPanel.SetActive(false);
    }

    private void ShowBuildingButtons()
    {
        int i = 0;
        for (; i < buildingsAvailable.Count(); ++i) {
            BuildingType buildingType = buildingsAvailable[i];
            Sprite sprite = Resources.Load("UI/Icons/" + buildingInfos[(int)buildingType].Icon, typeof(Sprite)) as Sprite;
            buyingButtons[i].GetComponent<Image>().sprite = sprite;
            string buttonName = buildingType.ToString();
            buttonName = buttonName.First().ToString() + buttonName.Substring(1).ToLower();
            buyingButtons[i].gameObject.SetActive(true);
            nameTextsButton[i].text = buttonName;
            if (!selectedCity.HasBuilding(buildingType)) {
                costTextsButton[i].text = buildingInfos[(int)buildingType].ShieldCost + " shields";
            }
            else {
                costTextsButton[i].text = "Built already";
            }
        }

        for (; i < TOTAL_BUY_BUTTONS; ++i) {
            buyingButtons[i].gameObject.SetActive(false);
        }

        showUnitsButton.gameObject.SetActive(true);
        showBuildingsButton.gameObject.SetActive(false);
    }

    private void ShowUnitButtons()
    {
        int i = 0;
        for (; i < unitsAvailable.Count(); ++i) {
            UnitType unitType = unitsAvailable[i];
            Sprite sprite = Resources.Load("UI/Icons/" + unitStats[(int)unitType].Icon, typeof(Sprite)) as Sprite;
            buyingButtons[i].GetComponent<Image>().sprite = sprite;
            string buttonName = unitType.ToString();
            buttonName = buttonName.First().ToString() + buttonName.Substring(1).ToLower();
            buyingButtons[i].gameObject.SetActive(true);
            nameTextsButton[i].text = buttonName;
            costTextsButton[i].text = unitStats[(int)unitType].ShieldCost + " shields";
        }

        for (; i < TOTAL_BUY_BUTTONS; ++i) {
            buyingButtons[i].gameObject.SetActive(false);
        }
        showUnitsButton.gameObject.SetActive(false);
        showBuildingsButton.gameObject.SetActive(true);
    }
}

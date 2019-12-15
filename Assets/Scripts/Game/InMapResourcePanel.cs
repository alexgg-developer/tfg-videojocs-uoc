using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static BuildingInfo;
using static TechnologyInfo;
using static UnitStats;


public class InMapResourcePanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    Text descriptionText;
    [SerializeField]
    Button collectButton;
    [SerializeField]
    Text textButton;
    [SerializeField]
    ResourceController resourceController;
#pragma warning restore 0649

    const string NOT_ENOUGH_SHIELDS = "Not enough shields.";
    const string DEFAULT_TEXT = "This is a resource. You can pay 10 shields to collect the resource for the nearest city that will grow by one.\n\n";
    const string NO_CITY = "Currently there is no city that can collect it (has to be to one tile of distance from a city).";
    const string CITY = "The nearest city is ";
    const string NO_TECHNOLOGY = "You don't have the technology to collect this resource yet.";

    bool hasTechnology = false;
    public bool HasTechnology
    {
        get
        {
            return hasTechnology;
        }
        set
        {
            hasTechnology = value;
        }
    }

    bool hasMoney = false;
    public bool HasMoney
    {
        get
        {
            return hasMoney;
        }
        set
        {
            hasMoney = value;
        }
    }

    bool isCityNear = false;
    public bool IsCityNear
    {
        get
        {
            return isCityNear;
        }
        set
        {
            isCityNear = value;
        }
    }

    string nearestCity = "";
    public string NearestCity
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

    Logic logic;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
    }

    private void OnEnable()
    {
        if(!hasTechnology) {
            //collectButton.gameObject.SetActive(false);
            collectButton.interactable = false;
            descriptionText.text = DEFAULT_TEXT + NO_TECHNOLOGY;
        }
        else if(!hasMoney) {
            //collectButton.gameObject.SetActive(false);
            collectButton.interactable = false;
            descriptionText.text = DEFAULT_TEXT + NOT_ENOUGH_SHIELDS;
        }
        else if(!IsCityNear) {
            //collectButton.gameObject.SetActive(false);
            collectButton.interactable = false;
            descriptionText.text = DEFAULT_TEXT + NO_CITY;
        }
        else {
            //collectButton.gameObject.SetActive(true);
            collectButton.interactable = true;
            descriptionText.text = DEFAULT_TEXT + CITY + nearestCity + ".";
        }
    }

    private void OnDisable()
    {
    }

    public void OnClickedCollectButton()
    {
        resourceController.CollectResource();
    }
}

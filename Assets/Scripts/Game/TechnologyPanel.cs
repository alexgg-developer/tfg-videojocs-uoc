using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static BuildingInfo;
using static TechnologyInfo;
using static UnitStats;


public class TechnologyPanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    GameObject descriptionSelectedPanel;
    [SerializeField]
    Text descriptionSelectedText;
    [SerializeField]
    Button[] buyingButtons;
    [SerializeField]
    Text[] nameTextsButton;
    [SerializeField]
    Text[] costTextsButton;
    [SerializeField]
    TechnologyInfo[] technologyInfo;
    [SerializeField]
    UnityEvent OnOpenEvent;
#pragma warning restore 0649
    const string NOT_ENOUGH_SHIELDS = "Not enough shields.";

    Logic logic;
    int selectedButton = -1;

    List<TechnologyType> technologiesAvailable = new List<TechnologyType>();

    private void Awake()
    {
        //ShowUnitButtons();
        technologiesAvailable.Add(TechnologyType.AGRICULTURE);
        technologiesAvailable.Add(TechnologyType.ENGINEERING);
        technologiesAvailable.Add(TechnologyType.NAVIGATION);
    }

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        ShowTechnologyButtons();
    }

    private void OnEnable()
    {
        OnOpenEvent.Invoke();
    }

    private void OnDisable()
    {
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
        TechnologyType technologyType = technologiesAvailable[index];
        descriptionSelectedText.text = technologyInfo[(int)technologyType].Description;

        if (selectedButton == index && !logic.HasTechnology(technologyType)) {
            if (logic.IsThereEnoughShields(technologyInfo[(int)technologyType].ShieldCost)) {
                logic.AddTechnology(technologyType);
                logic.TrySpendShields(technologyInfo[(int)technologyType].ShieldCost);
                ShowTechnologyButtons();
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


    private void ShowTechnologyButtons()
    {
        int i = 0;
        for (; i < technologiesAvailable.Count(); ++i) {
            TechnologyType technologyType = technologiesAvailable[i];
            Sprite sprite = Resources.Load("UI/Icons/" + technologyInfo[(int)technologyType].Icon, typeof(Sprite)) as Sprite;
            buyingButtons[i].GetComponent<Image>().sprite = sprite;
            string buttonName = technologyType.ToString();
            buttonName = buttonName.First().ToString() + buttonName.Substring(1).ToLower();
            buyingButtons[i].gameObject.SetActive(true);
            nameTextsButton[i].text = buttonName;
            if (!logic.HasTechnology(technologyType)) {
                costTextsButton[i].text = technologyInfo[(int)technologyType].ShieldCost + " shields";
            }
            else {
                costTextsButton[i].text = "Researched already";
            }
        }

    }
}

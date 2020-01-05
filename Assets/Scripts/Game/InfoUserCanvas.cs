using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoUserCanvas : MonoBehaviour
{

#pragma warning disable 0649
    [SerializeField]
    GameObject technologyPanel;
    [SerializeField]
    GameObject cityPanel;
    [SerializeField]
    GameObject menuPanel;
    [SerializeField]
    GameObject inMapResourcePanel;
    [SerializeField]
    GameObject controlsPanel;
    [SerializeField]
    GameObject unitPanel;
    [SerializeField]
    Button buttonTechnologyPanel;
    [SerializeField]
    Button buttonMenuPanel;
#pragma warning restore 0649

    Logic logic = null;

    private void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
    }

    public void ClosePanels()
    {
        technologyPanel.SetActive(false);
        cityPanel.SetActive(false);
        menuPanel.SetActive(false);
        inMapResourcePanel.SetActive(false);
        controlsPanel.SetActive(false);
        unitPanel.SetActive(false);
    }

    public void SwitchTechnologyPanel()
    {
        ClosePanels();
        if (!logic.IsEndOfGame) {
            technologyPanel.SetActive(!technologyPanel.activeInHierarchy);
        }
    }

    public void SwitchMenuPanel()
    {
        ClosePanels();
        if (!logic.IsEndOfGame) {
            menuPanel.SetActive(!menuPanel.activeInHierarchy);
        }
    }

    public void SwitchControlPanel()
    {
        ClosePanels();
        if (!logic.IsEndOfGame) {
            controlsPanel.SetActive(!controlsPanel.activeInHierarchy);
        }
    }

    public void OpenInMapResourcePanel()
    {
        ClosePanels();
        if (!logic.IsEndOfGame) {
            inMapResourcePanel.SetActive(true);
        }
    }

    public void OnChangeOfPlayer(int newPlayerID)
    {
        //technologyPanel.SetActive(false);
        //cityPanel.SetActive(false);
        ClosePanels();
        if(logic.IsCurrentPlayerAI) {
            buttonMenuPanel.interactable = false;
            buttonTechnologyPanel.interactable = false;
        }
        else {
            buttonMenuPanel.interactable = true;
            buttonTechnologyPanel.interactable = true;
        }
    }

    public void OnTechnologyPanelOpen()
    {
        cityPanel.SetActive(false);
    }

    public void OnCityPanelOpen()
    {
        technologyPanel.SetActive(false);
    }

    public void OpenUnitPanel(Unit selectedUnit)
    {
        ClosePanels();
        if (!logic.IsEndOfGame) {
            unitPanel.GetComponent<UnitPanel>().SelectedUnit = selectedUnit;
            unitPanel.SetActive(true);
        }
    }

    internal void CloseUnitPanel()
    {
        if (unitPanel.activeInHierarchy) {
            unitPanel.SetActive(false);
        }
    }

    public void UpdateUnitPanel()
    {
        unitPanel.GetComponent<UnitPanel>().UpdateInfo();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoUserCanvas : MonoBehaviour
{
    [SerializeField]
    GameObject technologyPanel;
    [SerializeField]
    GameObject cityPanel;
    [SerializeField]
    GameObject menuPanel;

    Logic logic = null;

    private void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
    }

    public void SwitchTechnologyPanel()
    {
        if (!logic.IsEndOfGame) {
            technologyPanel.SetActive(!technologyPanel.activeInHierarchy);
        }
    }

    public void SwitchMenuPanel()
    {
        if (!logic.IsEndOfGame) {
            menuPanel.SetActive(!menuPanel.activeInHierarchy);
        }
    }

    public void OnChangeOfPlayer(int newPlayerID)
    {
        technologyPanel.SetActive(false);
        cityPanel.SetActive(false);
    }

    public void OnTechnologyPanelOpen()
    {
        cityPanel.SetActive(false);
    }
    
    public void OnCityPanelOpen()
    {
        technologyPanel.SetActive(false);
    }
}

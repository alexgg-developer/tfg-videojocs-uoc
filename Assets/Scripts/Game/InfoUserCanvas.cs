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

    public void SwitchTechnologyPanel()
    {
        technologyPanel.SetActive(!technologyPanel.activeInHierarchy);
    }

    public void SwitchMenuPanel()
    {
        menuPanel.SetActive(!menuPanel.activeInHierarchy);
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

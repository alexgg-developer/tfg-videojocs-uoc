using UnityEngine;

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

    public void OpenInMapResourcePanel()
    {
        ClosePanels();
        if (!logic.IsEndOfGame) {
            inMapResourcePanel.SetActive(true);
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

using UnityEngine;
using UnityEngine.UI;

public class ResourcesPanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    Text shieldsText;
#pragma warning restore 0649
    Logic logic;
    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();        
    }
    
    //TO DO: Remove callback
    public void OnChangeOfPlayer(int newPlayerID)
    {
       //shieldsText.text = logic.Players[newPlayerID].Shields + " shields (+" + logic.Players[newPlayerID].CalculateShieldsPerTurn() + " per turn)";
    }

    // Update is called once per frame
    void Update()
    {
        shieldsText.text = logic.Players[logic.CurrentPlayer].Shields + " shields (+" + logic.Players[logic.CurrentPlayer].CalculateShieldsPerTurn() + " per turn)";
    }
}

using UnityEngine;
using UnityEngine.UI;

public class TurnPanel : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    Text turnText;
#pragma warning restore 0649
    Logic logic;
    int totalTurns;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        totalTurns = PlayerPrefs.GetInt("Turns");
        turnText.text = "Turn 1 of " + totalTurns;
    }

    public void OnChangeOfTurn(int newTurn)
    {
        turnText.text = "Turn " + newTurn + " of " + totalTurns;
    }
}

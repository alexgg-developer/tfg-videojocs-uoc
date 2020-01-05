using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    List<int> turnValues = new List<int>();
    public Toggle toggleIAPlayer;
    public Dropdown dropdownTurns;

    private void Start()
    {
        turnValues.Add(60);
        turnValues.Add(120);
        turnValues.Add(240);
    }

    private void OnEnable()
    {
        bool isIAEnabled = PlayerPrefs.GetInt("IAPlayer") == 1 ? true : false;
        toggleIAPlayer.isOn = isIAEnabled;
        int turns = PlayerPrefs.GetInt("Turns");
        switch(turns) {
            case 60:
                dropdownTurns.value = 0;
                break;
            case 120:
                dropdownTurns.value = 1;
                break;
            case 240:
                dropdownTurns.value = 2;
                break;
        }

    }
    public void TogglePlayWithAI(bool value)
    {      
        PlayerPrefs.SetInt("IAPlayer", value ? 1 : 0);
    }

    public void ChangedTurnValue(int value)
    {
        PlayerPrefs.SetInt("Turns", turnValues[value]);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    List<int> turnValues = new List<int>();

    private void Start()
    {
        turnValues.Add(60);
        turnValues.Add(120);
        turnValues.Add(240);
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

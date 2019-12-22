using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptableObjects/TechnologyInfo")]
public class TechnologyInfo : ScriptableObject
{
    public string Icon;
    public string Description;
    public uint ShieldCost;

    public enum TechnologyType { AGRICULTURE, ENGINEERING, NAVIGATION, SIZE };
    public TechnologyType Type;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptableObjects/BuildingInfo")]
public class BuildingInfo : ScriptableObject
{
    public string Icon;
    public string Description;
    public uint ShieldCost;

    public enum BuildingType { DOCK, BARN, WALL, SIZE };
    public BuildingType Type;
}

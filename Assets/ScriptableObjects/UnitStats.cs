using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomScriptableObjects/UnitStats")]
public class UnitStats : ScriptableObject
{
    public float Attack;
    public float Defense;
    public float Movement;
    public int Health;
    public string Icon;
    public string Description;
    public uint ShieldCost;

    public enum UnitType { WARRIOR, SETTLER, HORSEMAN, SHIP, ARCHER, CATAPULT, SIZE };
    public enum UnitAction { HEAL, BUILD_CITY, SIZE };

    public UnitType Type;
    public UnitAction Action;
    //public List<UnitAction> ActionsUnit = new List<UnitAction>();
}

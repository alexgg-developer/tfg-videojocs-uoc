using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameUnits/UnitStats")]
public class UnitStats : ScriptableObject
{
    public float Attack;
    public float Defense;
    public float Movement;
    public int Health;

    public enum UnitType { WARRIOR, SETTLER, HORSEMAN, SHIP, ARCHER, CATAPULT, SIZE };
    public UnitType Type;
}

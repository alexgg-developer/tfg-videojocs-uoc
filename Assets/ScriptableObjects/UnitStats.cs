using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameUnits/UnitStats")]
public class UnitStats : ScriptableObject
{
    public int Attack;
    public int Defense;
    public float Movement;
    public int Health;

    public enum UnitType { WARRIOR, SETTLER, SIZE };
    public UnitType Type;
}

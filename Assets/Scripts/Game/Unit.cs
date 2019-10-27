using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitStats;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Unit : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private UnitStats unitStats;
    public int Attack { get { return unitStats.Attack; } }
    public UnitType Type { get { return unitStats.Type; } }
    private Vector3 gridPosition = new Vector3(0f, 0f, 0f);
    private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }
    private int playerID;
    public int PlayerID { get { return playerID; } set { playerID = value; } }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = unitStats.Health;
    }
}

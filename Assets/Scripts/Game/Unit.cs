using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Unit : MonoBehaviour
{
    [SerializeField]
    private UnitStats unitStats;
    public int Attack { get { return unitStats.Attack; }}
    private Vector3 gridPosition = new Vector3(0f, 0f, 0f);
    private int currentHealth;
    public int CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = unitStats.Health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnitStats;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Unit : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private UnitStats unitStats;
    [SerializeField]
    private Image healthBar;

    private static int nextID = 0;

    private int id;
    public int ID { get { return id; } set { id = value; } }
    public float Attack { get { return unitStats.Attack; } }
    public float Defense { get { return unitStats.Defense; } }
    public int Health { get { return unitStats.Health; } }
    public float Movement { get { return unitStats.Movement; } }
    public UnitType Type { get { return unitStats.Type; } }
    //public List<UnitAction> ActionsUnit { get { return unitStats.ActionsUnit; } }
    public UnitAction Action { get { return unitStats.Action; } }
    private int currentHealth;
    public int CurrentHealth {
        get { return currentHealth; }
        set {
            currentHealth = value;
            healthBar.fillAmount = (float)currentHealth / (float)unitStats.Health;
        }
    }
    private int playerID;
    public int PlayerID { get { return playerID; } set { playerID = value; } }
    private float movementLeft;
    public float MovementLeft { get { return movementLeft; } set { movementLeft = value; } }    
    private bool hasAttacked = false;
    public bool HasAttacked { get { return hasAttacked; } set { hasAttacked = value; } }
    
    private Vector3 gridPosition = new Vector3(0f, 0f, 0f);

    //public const uint MAX_HEALTH = 10;
    public const int HEAL_RATIO = 3;

    void Awake()
    {
        ID = nextID++;
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = unitStats.Health;
        movementLeft = unitStats.Movement;
    }

    public void Reset()
    {
        movementLeft = unitStats.Movement;
        hasAttacked = false;
    }

    public void OnNewPosition()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}


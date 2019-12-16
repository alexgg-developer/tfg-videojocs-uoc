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

    private static int nextID = 0;

    private int id;
    public int ID { get { return id; } set { id = value; } }

    public float Attack { get { return unitStats.Attack; } }
    public float Defense { get { return unitStats.Defense; } }
    public UnitType Type { get { return unitStats.Type; } }
    private Vector3 gridPosition = new Vector3(0f, 0f, 0f);
    private int currentHealth;
    public int CurrentHealth {
        get { return currentHealth; }
        set {
            currentHealth = value;
            healthStatus.text = currentHealth + "/" + unitStats.Health;
        }
    }
    private int playerID;
    public int PlayerID { get { return playerID; } set { playerID = value; } }
    private float movementLeft;
    public float MovementLeft { get { return movementLeft; } set { movementLeft = value; } }    
    private bool hasAttacked = false;
    public bool HasAttacked { get { return hasAttacked; } set { hasAttacked = value; } }

    private Text healthStatus;
    public Text HealthStatus { get { return healthStatus; } set { healthStatus = value; } }

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
        float offsetX = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x * gameObject.transform.localScale.x * 0.25f;
        float offsetY = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.y * gameObject.transform.localScale.y * 2.0f;
        healthStatus.transform.position = new Vector3(gameObject.transform.position.x + offsetX, gameObject.transform.position.y + offsetY, gameObject.transform.position.z);        
    }

    private void OnDestroy()
    {
        GameObject.Destroy(healthStatus);
    }
}


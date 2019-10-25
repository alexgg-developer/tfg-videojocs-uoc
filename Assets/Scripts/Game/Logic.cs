using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private UnitManager unitManager;
    [SerializeField]
    private RandomHexGridCreator gridCreator;

    // Start is called before the first frame update
    void Start()
    {
        unitManager = gameObject.GetComponent<UnitManager>();
        gridCreator.CreateGrid();
        unitManager.InstantiateIntialUnits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

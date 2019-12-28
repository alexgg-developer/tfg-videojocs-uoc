using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BuildingInfo;
using static ScoreManager;
using static TechnologyInfo;
using static UnitStats;


public class EditorLogic : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private HexMapEditor gridCreator;
#pragma warning restore 0649


    // Start is called before the first frame update
    void Start()
    {       
        gridCreator.CreateGrid();        
    }
}

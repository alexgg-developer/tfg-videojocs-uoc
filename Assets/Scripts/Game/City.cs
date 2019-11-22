using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public const uint SHIELDS_PER_POPULATION = 2;

    private uint population = 1;
    public uint Population { get { return population; } set { population = value; } }
    private int playerID;
    public int PlayerID { get { return playerID; } set { playerID = value; } }
}

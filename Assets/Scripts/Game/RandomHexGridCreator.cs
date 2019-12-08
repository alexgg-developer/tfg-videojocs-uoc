using UnityEngine;
using UnityEngine.EventSystems;
using static HexGrid;

public class RandomHexGridCreator : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private HexGrid hexGrid;
    public HexGrid HexGrid { get { return hexGrid; } }
    public int chunksX, chunksZ;
    public int MaxElevation;

    Color[] colors;

    void Awake()
    {
        colors = new Color[(int)TerrainTypes.SIZE];
        colors[(int)TerrainTypes.GRASS] = Color.green;
        colors[(int)TerrainTypes.ICE] = Color.white;
        colors[(int)TerrainTypes.MOUNTAIN] = new Color(178f / 255f, 115f / 255f, 65f / 255f);
        colors[(int)TerrainTypes.PLAIN] = Color.yellow;
        colors[(int)TerrainTypes.WATER] = Color.cyan;
    }

    public void CreateGrid()
    {
        //these are chunks
        hexGrid.CreateCells(chunksX, chunksZ);
        HexCell[] hexCells = hexGrid.Cells;
        int cellsInX = chunksX * HexMetrics.chunkSizeX;
        for (int i = 0; i < chunksZ * HexMetrics.chunkSizeZ; ++i) {
            for (int j = 0; j < cellsInX; ++j) {
                hexCells[i * cellsInX + j].color = colors[Random.Range(0, (int)TerrainTypes.SIZE)];
                hexCells[i * cellsInX + j].Elevation = Random.Range(0, MaxElevation);
            }
        }
        hexGrid.Refresh();
    }

    public HexCell GetCell(HexCoordinates hexCoordinates)
    {
        return hexGrid.GetCell(hexCoordinates.X, hexCoordinates.Z);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using static HexGrid;

public class RandomHexGridCreator : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private HexGrid hexGrid;
    public HexGrid HexGrid { get { return hexGrid; } }
    public int Width, Height;
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
        hexGrid.CreateGrid(Width, Height);
        HexCell[] hexCells = hexGrid.Cells;
        for (int i = 0; i < Height; ++i) {
            for (int j = 0; j < Width; ++j) {
                hexCells[i * Width + j].color = colors[Random.Range(0, (int)TerrainTypes.SIZE)];
                hexCells[i * Width + j].Elevation = Random.Range(0, MaxElevation);
            }
        }
        hexGrid.Refresh();
    }
}

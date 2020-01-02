using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using static HexGrid;

public class HexGridLoader : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private HexGrid hexGrid;
    public HexGrid HexGrid { get { return hexGrid; } }
    public int chunksX, chunksZ;
    
    void Awake()
    {
    }

    public void CreateGrid()
    {
        //these are chunks
        hexGrid.CreateCells(chunksX, chunksZ);
        Load();
    }

    public HexCell GetCell(HexCoordinates hexCoordinates)
    {
        return hexGrid.GetCell(hexCoordinates.X, hexCoordinates.Z);
    }

    public void Load()
    {
        //string path = Path.Combine(Application.persistentDataPath, "test.map");
        string path = Path.Combine("", "test.map");
        using (BinaryReader reader =
                new BinaryReader(File.OpenRead(path))
        ) {
            hexGrid.Load(reader);
        }
    }
}

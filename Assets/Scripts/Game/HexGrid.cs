using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    [SerializeField]
    private HexGridChunk chunkPrefab;
    [SerializeField]
    private Texture2D noiseSource;
    //[SerializeField]
	//this is now calculated
    private int cellCountX;
    public int CellCountX { get { return cellCountX; } private set { cellCountX = value; } }
    //[SerializeField]
    private int cellCountZ;
    public int CellCountZ { get { return cellCountZ; } private set { cellCountZ = value; } }

    [SerializeField]
    public int chunkCountX, chunkCountZ;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    public enum TerrainTypes { GRASS, PLAIN, ICE, MOUNTAIN, WATER, SIZE }

    private HexCell[] cells;
    public HexCell[] Cells { get { return cells; } private set { cells = value; } }
    HexGridChunk[] chunks;

    void Awake()
    {
        HexMetrics.noiseSource = noiseSource;

        //CreateChunks();
        //CreateCells();
    }

    /*void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++) {
            for (int x = 0; x < chunkCountX; x++) {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }*/

    void CreateChunks(int givenChunkCountX, int givenChunkCountZ)
    {
        chunkCountX = givenChunkCountX;
        chunkCountZ = givenChunkCountZ;

        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++) {
            for (int x = 0; x < chunkCountX; x++) {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    public void CreateCells(int givenChunkCountX, int givenChunkCountZ)
    {
        CreateChunks(givenChunkCountX, givenChunkCountZ);

        cellCountX = givenChunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = givenChunkCountZ * HexMetrics.chunkSizeZ;

        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++) {
            for (int x = 0; x < cellCountX; x++) {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        //label.text = x.ToString() + "\n" + z.ToString();
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;

        if (x > 0) {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }

        if (z > 0) {
            if ((z & 1) == 0) {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0) {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1) {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];


        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);

    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(int coordinateX, int coordinateZ)
    {
        int index = coordinateX + coordinateZ * cellCountX + coordinateZ / 2;
        return cells[index];
    }

    public HexCell GetCell(int index)
    {
        return cells[index];
    }

    /*public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }*/

    public void Refresh()
    {
        //hexMesh.Triangulate(cells);
        for (int z = 0; z < chunkCountZ; z++) {
            for (int x = 0; x < chunkCountX; x++) {
                chunks[z * chunkCountX + x].Refresh();
            }
        }
    }
}
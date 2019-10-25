﻿using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;

    public enum TerrainTypes { GRASS, PLAIN, ICE, MOUNTAIN, WATER, SIZE }

    [SerializeField]
    private int width = 6;
    public int Width { get { return width; } private set { width = value; } }
    [SerializeField]
    private int height = 6;
    public int Height { get { return height; } private set { height = value; } }
    private HexCell[] cells;
    public HexCell[] Cells { get { return cells; } private set { cells = value; } }
    private Canvas gridCanvas;
    private HexMesh hexMesh;


    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
    }

    public void CreateGrid(int width, int height)
    {

        Width = width;
        Height = height;
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z, i++);
            }
        }
    }
    
    void Start()
    {
        //hexMesh.Triangulate(cells);
    }


    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        //label.text = x.ToString() + "\n" + z.ToString();
        label.text = cell.coordinates.ToStringOnSeparateLines();

        if (x > 0) {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }

        if (z > 0) {
            if ((z & 1) == 0) {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0) {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            else {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1) {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }
        cell.uiRect = label.rectTransform;
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(int offsetX, int offsetY)
    {
        int index = offsetX + offsetY * width + offsetY / 2;
        return cells[index];
    }

    public void Refresh()
    {
        hexMesh.Triangulate(cells);
    }
}
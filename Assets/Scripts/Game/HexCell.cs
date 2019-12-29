using UnityEngine;
using UnityEngine.UI;
using static HexMetrics;

public class HexCell : MonoBehaviour
{
    static private Color[] highLightColors =
    {

        new Color(0f, 0f, 1.0f),
        new Color(1.0f, 0f, 0.0f)
    };

    public HexCoordinates coordinates;
    public HexGridChunk chunk;

    private Color color;
    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
        }
    }

    int elevation = 0;
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                HexMetrics.elevationPerturbStrength;

            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;

            if (hasOutgoingRiver &&
                elevation < GetNeighbor(outgoingRiver).elevation) {
                RemoveOutgoingRiver();
            }
            if (hasIncomingRiver &&
                elevation > GetNeighbor(incomingRiver).elevation) {
                RemoveIncomingRiver();
            }

            Refresh();
        }
    }
    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }
    private bool hasResource = false;
    public bool HasResource
    {
        get
        {
            return hasResource;
        }
        set
        {
            hasResource = value;
        }
    }

    public RectTransform uiRect;

    private GameObject resource;

    #region rivers
    HexDirection incomingRiver, outgoingRiver;
    bool hasIncomingRiver;
    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }

    bool hasOutgoingRiver;
    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }

    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd
    {
        get
        {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return
            hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }

    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver) {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver) {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction) {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.elevation) {
            return;
        }
        //this is needed bc it works with neighbor too.
        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction) {
            RemoveIncomingRiver();
        }
        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly();

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        neighbor.RefreshSelfOnly();
    }

    public float StreamBedY
    {
        get
        {
            return
                (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    #endregion


#pragma warning disable 0649
    [SerializeField]
    private HexCell[] neighbors;
#pragma warning restore 0649

    public void Start()
    {
        /*color = new Color(
          Random.Range(0f, 1f),
          Random.Range(0f, 1f),
          Random.Range(0f, 1f)
      );*/
    }

    public HexCell[] GetNeighbors()
    {
        return neighbors;
    }

    public HexCell GetRandomNeighbor()
    {
        int direction = Random.Range(0, 5);

        return neighbors[direction];
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public void EnableHighlight(int playerID)
    {
        Color highLightColor = highLightColors[playerID];
        EnableHighlight(highLightColor);
    }

    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    public void SetResource(bool activate)
    {
        if (activate) {
            resource = Instantiate(Resources.Load("Corn", typeof(GameObject)), gameObject.transform) as GameObject;
            float offsetY = resource.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y * resource.transform.localScale.y * 0.5f;
            resource.transform.Translate(new Vector3(0f, offsetY, 0f));
            hasResource = true;
        }
        else {
            if (resource != null) {
                Destroy(resource);
                resource = null;
                hasResource = false;
            }
        }
    }

    void Refresh()
    {
        if (chunk) {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++) {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk) {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }


    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
}
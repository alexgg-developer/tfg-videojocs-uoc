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
    public Color color;
    int elevation;
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

#pragma warning disable 0649
    [SerializeField]
    private HexCell[] neighbors;

    public void Start()
    {
        /*color = new Color(
          Random.Range(0f, 1f),
          Random.Range(0f, 1f),
          Random.Range(0f, 1f)
      );*/
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
            if(resource != null) {
                Destroy(resource);
                resource = null;
                hasResource = false;
            }
        }
    }
}
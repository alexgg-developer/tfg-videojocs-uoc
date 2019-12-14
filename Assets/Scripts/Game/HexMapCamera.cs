using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    [SerializeField]
    private HexGrid grid;

    Transform swivel, stick;
    float zoom = 0.7f;
    float rotationAngle;

    public float stickMinZoom, stickMaxZoom;
    public float swivelMinZoom, swivelMaxZoom;
    //  public float moveSpeed = 100;
    public float moveSpeedMinZoom = 400, moveSpeedMaxZoom = 100;
    public float rotationSpeed = 180;

    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);

        UpdateZoom();
    }

    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f) {
            AdjustZoom(zoomDelta);
        }
        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f) {
            AdjustRotation(rotationDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f) {
            AdjustPosition(xDelta, zDelta);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);
        UpdateZoom();
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f) {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f) {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    void UpdateZoom()
    {
        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        Debug.Log("Damping " + damping);
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;
        Vector3 direction = transform.localRotation *
            new Vector3(xDelta, 0f, zDelta).normalized;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f )*
            (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 60.0f, xMax - 60.0f);

        float zMax =
            (grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) *
            (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 40f, zMax - 40.0f);

        return position;
    }
}
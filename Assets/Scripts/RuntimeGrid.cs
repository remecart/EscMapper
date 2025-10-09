using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RuntimeGrid : MonoBehaviour
{
    public int gridSizeX;

    public int gridSizeY;

    public float cellSize = 1f;

    private LineRenderer lineRenderer;

    public string sortingLayerName = "Default";

    public int sortingOrder;


    public Camera cam;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.sortingLayerName = sortingLayerName;
        lineRenderer.sortingOrder = sortingOrder;
        toggle = FolderPath.instance.Config.viewGrid;
        DrawLines();
    }

    public bool toggle;

    public void UpdateGridStatus()
    {
        toggle = FolderPath.instance.viewGrid.isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            toggle = !toggle;
            FolderPath.instance.Config.viewGrid = toggle;
        }

        if (toggle)
        {
            float lineWidth = 1f * (cam.orthographicSize * 2f / cam.pixelHeight);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;   
        }
        else
        {
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;   
        }
    }

    private void DrawLines()
    {
        int verticalPairs = Mathf.CeilToInt(gridSizeX / 2f);
        int horizontalPairs = Mathf.CeilToInt(gridSizeY / 2f);

        lineRenderer.positionCount = (verticalPairs * 4 + horizontalPairs * 4) + 5;
        int num = 0;

        // Vertical double lines
        for (int i = 0; i < verticalPairs; i++)
        {
            float x = (i * 2) * cellSize;
            lineRenderer.SetPosition(num++, new Vector2(x, 0f));
            lineRenderer.SetPosition(num++, new Vector2(x, gridSizeY * cellSize));

            lineRenderer.SetPosition(num++, new Vector2(x + 1f, gridSizeY * cellSize));
            lineRenderer.SetPosition(num++, new Vector2(x + 1f, 0f));
        }
        
        lineRenderer.SetPosition(num++, new Vector2(0f, 0));
        
        // Horizontal double lines
        for (int j = 0; j < horizontalPairs; j++)
        {
            float y = (j * 2) * cellSize + 1f;
            lineRenderer.SetPosition(num++, new Vector2(0f, y));
            lineRenderer.SetPosition(num++, new Vector2(gridSizeX * cellSize, y));

            lineRenderer.SetPosition(num++, new Vector2(gridSizeX * cellSize, y - 1f));
            lineRenderer.SetPosition(num++, new Vector2(0f, y - 1f));
        }
        
        lineRenderer.SetPosition(num++, new Vector2(0f, 0));
        lineRenderer.SetPosition(num++, new Vector2(gridSizeX, 0));
        lineRenderer.SetPosition(num++, new Vector2(gridSizeX, gridSizeY));
        lineRenderer.SetPosition(num++, new Vector2(gridSizeX, gridSizeY));

        num++;

    }
}
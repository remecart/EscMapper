using System.Collections.Generic;
using UnityEngine;

public class PerimeterVisualizer : MonoBehaviour
{
    public static PerimeterVisualizer instance;
    public List<GameObject> perimeters;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    private Vector3 mousePos;
    private GameObject go;

    private void Update()
    {
        if (MapProperties.instance.IsEmpty())
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            foreach (GameObject child in perimeters)
            {
                child.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            foreach (GameObject child in perimeters)
            {
                child.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        if (!Input.GetKey(KeyCode.Tab)) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = (Vector3)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider == null) return;
            
            go = hit.collider.gameObject;
            mousePos -= go.transform.position;
            if (!go.CompareTag("Perimeter")) return;
            
            
        }

        if (Input.GetMouseButton(0))
        {
            var currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (go == perimeters[0]) go.transform.position = new Vector3(go.transform.position.x, Mathf.Clamp(Mathf.RoundToInt(currentMousePos.y - mousePos.y), 501 - 108, 501), 0);
            else if (go == perimeters[1]) go.transform.position = new Vector3(go.transform.position.x, Mathf.Clamp(Mathf.RoundToInt(currentMousePos.y - mousePos.y), -607, -607 + 108));
            else if (go == perimeters[2]) go.transform.position = new Vector3(Mathf.Clamp(Mathf.RoundToInt(currentMousePos.x - mousePos.x), 608 - 108, 608), go.transform.position.y, 0);
            else if (go == perimeters[3]) go.transform.position = new Vector3(Mathf.Clamp(Mathf.RoundToInt(currentMousePos.x - mousePos.x), -500, -500 + 108), go.transform.position.y, 0);

            UpdatePerimFromVisuals();
        }
    }
    
    public void UpdatePerimFromVisuals()
    {
        if (MapProperties.instance?.properties?.Perim == null) return;

        MapProperties.instance.properties.Perim.Top = Mathf.RoundToInt(502 - perimeters[0].transform.position.y);
        MapProperties.instance.properties.Perim.Bottom = Mathf.RoundToInt(perimeters[1].transform.position.y + 606);
        MapProperties.instance.properties.Perim.Right = Mathf.RoundToInt(608 - perimeters[2].transform.position.x);
        MapProperties.instance.properties.Perim.Left = Mathf.RoundToInt(perimeters[3].transform.position.x + 500);
    }

    public void Visualize()
    {
        var perim = MapProperties.instance.properties.Perim;

        if (perim == null) return;
        
        int top = perim.Top != null ? perim.Top : 0;
        int bottom = perim.Bottom != null ? perim.Bottom : 0;
        int right = perim.Right != null ? perim.Right : 0;
        int left = perim.Left != null ? perim.Left : 0;
        
        float topY = Mathf.Clamp(502 - top, 501 - 108, 501);
        perimeters[0].transform.position = new Vector3(50, topY, 0);
        
        float bottomY = Mathf.Clamp(-606 + bottom, -607, -607 + 108);
        perimeters[1].transform.position = new Vector3(50, bottomY, 0);
        
        float rightX = Mathf.Clamp(608 - right, 608 - 108, 608);
        perimeters[2].transform.position = new Vector3(rightX, -54, 0);

        float leftX = Mathf.Clamp(-500 + left, -500, -500 + 108);
        perimeters[3].transform.position = new Vector3(leftX, -54, 0);
    }
}


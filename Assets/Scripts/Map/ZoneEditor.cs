using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ZoneEditor : MonoBehaviour
{
    public static ZoneEditor instance;
    private GameObject AreaPreview => PreviewArea.instance.gameObject;
    public Camera cam;
    private Vector3Int mousePos => TileEditor.instance.currentTilemap[1].WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
    public int selectedZoneIndex => dropdown.value;
    
    public GameObject ZoneParent;
    public TMP_Dropdown dropdown;
    public GameObject ZonePrefab;
    public List<string> ZoneNames;
    public List<string> availableNames;
    public List<Color> colors;
    
    private bool _clickedOnUI;
    private Vector3Int startPos;
    private Vector3 offset;
    private GameObject zone;
    private GameObject hoveringZone;
    private bool isDragging = false;
    
    private GameObject selectedZone;
    private Color selectedZoneColor = Color.white;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        RefreshAvailableZones();
        InvokeRepeating(nameof(SaveZones), 1f, 1f);
    }

    public void DeleteZones()
    {
        foreach (Transform Zone in ZoneParent.transform)
        {
            if (Zone.gameObject.name != "Bg") Destroy(Zone.gameObject);
        }
    }
    public void LoadZones()
    {
        var zonesData = MapProperties.instance.properties.Zones;
        var zoneCategories = typeof(Zones).GetFields();

        foreach (var categoryField in zoneCategories)
        {
            string categoryName = categoryField.Name;
            List<Zone> zoneList = categoryField.GetValue(zonesData) as List<Zone>;

            if (zoneList == null) continue;

            for (int i = 0; i < zoneList.Count; i++)
            {
                Zone zoneData = zoneList[i];
                Vector2 start = zoneData.startPos / 16f;
                Vector2 end = new Vector2(zoneData.endPos.x / 16f - 1, zoneData.endPos.y / 16f - 1);
                
                float xMin = Mathf.Min(start.x, end.x);
                float xMax = Mathf.Max(start.x, end.x);
                float yMin = Mathf.Min(start.y, end.y);
                float yMax = Mathf.Max(start.y, end.y);

                float width = xMax - xMin + 1;
                float height = yMax - yMin + 1;

                Vector3 center = new Vector3((xMin + xMax) / 2f + 0.5f, -((yMin + yMax) / 2f - 0.5f), 0f);
                GameObject zoneGO = Instantiate(ZonePrefab, center, Quaternion.identity, ZoneParent.transform);
                zoneGO.transform.localScale = new Vector3(width, height, 1f);
                zoneGO.name = $"{categoryName}";
                
                zoneGO.GetComponent<SpriteRenderer>().color = GetColor(categoryName);
            }
        }

        RefreshAvailableZones();
    }
    
    public void SaveZones()
    {
        var zonesData = new Zones();
        var zoneCategories = typeof(Zones).GetFields();
        
        Dictionary<string, List<Zone>> zoneDict = new Dictionary<string, List<Zone>>();

        foreach (Transform child in ZoneParent.transform)
        {
            string categoryName = child.name;
            
            Vector3 center = child.position;
            Vector3 scale = child.localScale;
            
            float xMin = center.x - scale.x / 2f;
            float xMax = center.x + scale.x / 2f + 1f;
            float yMin = -center.y - scale.y / 2f;
            float yMax = -center.y + scale.y / 2f + 1f;

            Vector2 startPos = new Vector2(xMin, yMin + 1) * 16f;
            Vector2 endPos = new Vector2(xMax - 1f, yMax) * 16f;

            Zone zone = new Zone
            {
                startPos = startPos,
                endPos = endPos
            };
            
            if (!zoneDict.ContainsKey(categoryName))
                zoneDict[categoryName] = new List<Zone>();

            zoneDict[categoryName].Add(zone);
        }
        
        foreach (var field in zoneCategories)
        {
            string categoryName = field.Name;
            if (zoneDict.TryGetValue(categoryName, out var zoneList))
            {
                field.SetValue(zonesData, zoneList);
            }
        }
        MapProperties.instance.properties.Zones = zonesData;
    }
    
    private Color GetColor(string zoneName)
    {
        var indexInList = ZoneNames
            .Select((name, index) => new { name, index })
            .FirstOrDefault(x => x.name == zoneName)?.index ?? -1;

        if (zoneName.Contains("Cell") || zoneName == "SHU") return colors[0];
        else if (zoneName.Contains("Safe")) return colors[1];
        else if (indexInList < 7) return colors[2];
        else return colors[3];
    }

    private void RefreshAvailableZones()
    {
        availableNames = new List<string>(ZoneNames);

        foreach (Transform child in ZoneParent.transform)
        {
            availableNames.Remove(child.gameObject.name);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(availableNames);
    }
    
    private void StopActions()
    {
        startPos = new();
        zone = null;
        AreaPreview.transform.localScale = new Vector3(0, 0, 0);
        isDragging = false;
    }
    
    public void DeleteSelectedZone()
    {
        if (selectedZone == null) return;

        Destroy(selectedZone);
        selectedZone = null;
        RefreshAvailableZones();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (mousePos.x < 0 || mousePos.x > 107 || mousePos.y > 0 || mousePos.y < -107)
        {
            StopActions();
            return;
        }
        
        if (MapProperties.instance.IsEmpty())
        {
            StopActions();
            return;
        }

        if (MapManager.instance.currentLayer != 4)  return;
        
        if (Input.GetKey(KeyCode.Tab))
        {
            StopActions(); 
            DeselectZone(); 
            return;
        }

        if ((PreventPlaceBehindGUI.instance.behindUI || _clickedOnUI) && selectedZone == null)
        {
            StopActions(); 
            DeselectZone(); 
            return;
        }

        if (!Input.GetMouseButton(0)) AreaPreview.transform.localScale = new Vector3(0, 0, 0);

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Zone"))
                {
                    SelectZone(hit.collider.gameObject);
                    offset = selectedZone.transform.position - mousePos;
                    break; 
                }
                else
                {
                    DeselectZone();
                    startPos = mousePos;
                    isDragging = true;
                }
            }
        }

        if (selectedZone != null)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteSelectedZone();
                return;
            }

            if (Input.GetMouseButton(0))
            {
                selectedZone.transform.position = mousePos + offset;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                DeselectZone();
            }

            return;
        }

        HandleZonePlacing(mouseWorld);
    }
    
    private void HandleZonePlacing(Vector3 mouseWorld)
    {
        RaycastHit2D hoverHit = Physics2D.Raycast(mouseWorld, Vector2.zero);
        if (hoverHit.collider != null && availableNames.Count != 0)
        {
            if (hoverHit.collider.gameObject.name == "Corner")
            {
                StopActions();
                return;
            }
            if (hoverHit.collider.gameObject.CompareTag("Zone"))
            {
                hoveringZone = hoverHit.collider.gameObject;
            }
        }
        else
        {
            hoveringZone = null;
        }

        if (Input.GetMouseButtonUp(0) && availableNames.Count != 0)
        {
            if (isDragging)
            {
                CreateNewZone();
                isDragging = false;
            }

            StopActions();
        }

        if (Input.GetMouseButton(0) && isDragging && availableNames.Count != 0)
        {
            PreviewArea.instance.AreaPreview(GetColor(dropdown.options[dropdown.value].text), startPos);
        }
    }
    
    
    private void SelectZone(GameObject zone)
    {
        DeselectZone(); // Deselect previous
        selectedZone = zone;
        offset = selectedZone.transform.position - mousePos;

        var sr = selectedZone.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = selectedZoneColor;
        }
    }

    private void DeselectZone()
    {
        if (selectedZone != null)
        {
            var sr = selectedZone.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = GetColor(selectedZone.name);
            }
        }

        selectedZone = null;
    }
    
    public void CreateNewZone()
    {
        var endAreaPos = mousePos;
    
        float width = Mathf.Abs(endAreaPos.x - startPos.x) + 1;
        float height = Mathf.Abs(endAreaPos.y - startPos.y) + 1;

        float xPos = ((float)startPos.x + (float)endAreaPos.x) / 2 + 0.5f;
        float yPos = ((float)startPos.y + (float)endAreaPos.y) / 2 + 0.5f;

        var go = Instantiate(ZonePrefab);
        go.transform.SetParent(ZoneParent.transform);

        go.GetComponent<SpriteRenderer>().color = GetColor(dropdown.options[dropdown.value].text);

        go.name = dropdown.options[selectedZoneIndex].text;

        go.transform.position = new Vector3(xPos, yPos, 0);
        go.transform.localScale = new Vector3(width, height, 1);
        
        RefreshAvailableZones();
    }
}

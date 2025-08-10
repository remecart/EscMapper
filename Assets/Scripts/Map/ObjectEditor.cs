using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectEditor : MonoBehaviour
{
    public static ObjectEditor instance;
    [FormerlySerializedAs("objects")] public List<GameObject> ObjectLayers;
    
    [Range(0, 111)]
    public int selectedObjectIndex;
    public int currentTilemapLayer => MapManager.instance.currentLayer;
    private Vector3Int mousePos =>
        TileEditor.instance.currentTilemap[currentTilemapLayer].WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
    public Camera cam;

    GameObject currentObject {
        get {
            return ObjectLookupTable.instance.objects[selectedObjectIndex];
        }
    }

    public void ChangeSelectedObject(int id) {
        selectedObjectIndex = id;
        TileEditor.instance.placementMode = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    private bool _clickedOnUI;
    
    // Update is called once per frame
    void Update()
    {
        if (currentTilemapLayer == 4) return;
        
        if (mousePos.x < 0 || mousePos.x > 107 || mousePos.y > 0 || mousePos.y < -107)
        {
            return;
        }
        
        if (MapProperties.instance.IsEmpty())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (GameObject layer in ObjectLayers)
            {
                layer.SetActive(false);
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            foreach (GameObject layer in ObjectLayers)
            {
                layer.SetActive(true);
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (PreventPlaceBehindGUI.instance.behindUI) _clickedOnUI = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _clickedOnUI = false;
        }
        
        if (Input.GetKey(KeyCode.Tab)) return;
        if (PreventPlaceBehindGUI.instance.behindUI) return;
        if (_clickedOnUI) return;
        if (Input.GetKey(KeyCode.LeftControl)) return;
        
        if (Input.GetMouseButton(1))
        {
            DeleteObject();
        }
        
        if (TileEditor.instance.placementMode) return;

        if (Input.GetMouseButton(0))
        {
            PlaceObject();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void AreaDeleteObject(Vector3Int startPos, Vector3Int endPos, List<UndoEntry> entries, bool paste = false)
    {
        int minX = Mathf.Min(startPos.x, endPos.x);
        int maxX = Mathf.Max(startPos.x, endPos.x);
        int minY = Mathf.Min(startPos.y, endPos.y);
        int maxY = Mathf.Max(startPos.y, endPos.y);

        foreach (Transform child in ObjectLayers[currentTilemapLayer].transform)
        {
            // Convert world position to grid position
            Vector3Int gridPos = Vector3Int.FloorToInt(child.transform.position);

            if (gridPos.x >= minX && gridPos.x <= maxX &&
                gridPos.y >= minY && gridPos.y <= maxY)
            {
                var co = child.GetComponent<CustomObject>();
                var entry = new UndoEntry
                {
                    isTile = false,
                    id = co.id,
                    layer = currentTilemapLayer,
                    position = child.position // exact world position
                };
                
                entries.Add(entry);
                    
                Destroy(child.gameObject);
            }
        }

        if (paste)
        {
            PasteObjects(entries);
        }
        else UndoRedoManager.instance.SaveState(entries);
    }
    
    public List<UndoEntry> copyEntries = new List<UndoEntry>();
    private Vector3 copyAnchorOffset;


    public void CopyObjects(Vector3Int startPos, Vector3Int endPos)
    {
        copyEntries.Clear();

        int minX = Mathf.Min(startPos.x, endPos.x);
        int maxX = Mathf.Max(startPos.x, endPos.x);
        int minY = Mathf.Min(startPos.y, endPos.y);
        int maxY = Mathf.Max(startPos.y, endPos.y);

        Vector3 anchor = new Vector3(endPos.x + 0.5f, endPos.y + 0.5f, 0); // use endPos as anchor
        copyAnchorOffset = anchor;

        foreach (Transform child in ObjectLayers[currentTilemapLayer].transform)
        {
            Vector3Int gridPos = Vector3Int.FloorToInt(child.transform.position);

            if (gridPos.x >= minX && gridPos.x <= maxX &&
                gridPos.y >= minY && gridPos.y <= maxY)
            {
                var co = child.GetComponent<CustomObject>();
                var entry = new UndoEntry
                {
                    isTile = false,
                    id = co.id,
                    layer = currentTilemapLayer,
                    position = child.position - anchor // store relative offset from endPos
                };

                copyEntries.Add(entry);
            }
        }
    }
    
    public void PasteObjects(List<UndoEntry> entries)
    {
        if (entries.Count == 0) return;
        
        Vector3 mouseWorldAnchor = new Vector3(mousePos.x + 0.5f, mousePos.y + 0.5f, 0);

        foreach (var entry in copyEntries)
        {
            Vector3 newPos = mouseWorldAnchor + entry.position; // entry.position is relative to original endPos

            // Prevent overlapping existing objects
            bool occupied = false;
            foreach (Transform child in ObjectLayers[entry.layer].transform)
            {
                if (Vector3.Distance(child.position, newPos) < 0.5f)
                {
                    occupied = true;
                    break;
                }
            }
            if (occupied) continue;

            GameObject prefab = ObjectLookupTable.instance.objects[entry.id];
            GameObject go = Instantiate(prefab);
            go.transform.SetParent(ObjectLayers[entry.layer].transform);
            go.transform.position = newPos;

            var co = go.GetComponent<CustomObject>();
            go.GetComponent<SpriteRenderer>().sortingOrder = entry.layer * 50 + 25;

            entries.Add(new UndoEntry
            {
                isTile = false,
                id = co.id,
                layer = entry.layer,
                position = newPos
            });
        }

        UndoRedoManager.instance.SaveState(entries);
    }
    
    void DeleteObject()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 worldPos = new Vector3(mousePos.x, mousePos.y, 0);

        Transform layer = ObjectLayers[currentTilemapLayer].transform;
        foreach (Transform child in layer)
        {
            if (Vector3.Distance(child.position, worldPos) < 0.5f)
            {
                var co = child.GetComponent<CustomObject>();
                if (co != null)
                {
                    UndoRedoManager.instance.SaveState(new List<UndoEntry> {
                        new UndoEntry {
                            isTile = false,
                            id = co.id,
                            layer = currentTilemapLayer,
                            position = child.position
                        }
                    });
                }

                Destroy(child.gameObject);
                break;
            }
        }
    }
    
    void PlaceObject(Objects obj = null)
    {
        Vector3 worldPos = new Vector3(mousePos.x + 0.5f, mousePos.y + 0.5f, 0);
        
        foreach (Transform child in ObjectLayers[currentTilemapLayer].transform)
        {
            if (child.transform.position == worldPos) return;
        }
        
        GameObject go = Instantiate(currentObject);
        go.transform.SetParent(ObjectLayers[currentTilemapLayer].transform);

        var co = go.GetComponent<CustomObject>();
        go.transform.position = new Vector3(mousePos.x + co.offset.x + 0.5f, mousePos.y + co.offset.y + 0.5f, 0);
        go.GetComponent<SpriteRenderer>().sortingOrder = currentTilemapLayer * 50 + 25;

        UndoRedoManager.instance.SaveState(new List<UndoEntry> {
            new UndoEntry {
                isTile = false,
                id = co.id,
                layer = currentTilemapLayer,
                position = go.transform.position
            }
        });
    }
}
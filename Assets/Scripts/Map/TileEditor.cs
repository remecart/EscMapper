using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEditor : MonoBehaviour
{
    public static TileEditor instance;
    public Camera cam;
    public List<Tilemap> currentTilemap;
    
    public int currentTilemapLayer => MapManager.instance.currentLayer;
    [Range(1, 100)]
    public int selectedTileIndex;

    public Vector3Int mousePos =>
        currentTilemap[Mathf.Clamp(currentTilemapLayer, 0, 3)].WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));

    public bool placementMode; // True = Tile-Placement / False = Object-Placement

    public Color placeColor;
    public Color deleteColor;
    public Color pasteColor;
    public Color copyColor;
    
    private TileBase CurrentTile => MapManager.instance.tiles[selectedTileIndex];

    public void ChangeSelectedTile(int newSelectedTileIndex)
    {
        selectedTileIndex = TileSelection.instance.ConvertXtoY(newSelectedTileIndex, 4, 25);
        placementMode = true;
        TileSelection.instance.UpdateBox();
    }

    public void Start()
    {
        instance = this;
    }

    private bool _clickedOnUI;
    private bool stopControls;

    public Vector3Int tempPos;
    
    private void StopActions()
    {
        startCopyPos = new();
        endCopyPos = new();
        copiedIds = new();
        startAreaPos = new();
    }
    
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftControl))
        {
            stopControls = false;
        }
        
        if (stopControls)
        {
            StopActions();
            return;
        }
        
        if (currentTilemapLayer == 4)
        {
            StopActions();
            return;
        }

        if (Input.GetMouseButtonUp(1) || tempPos != mousePos) tempPos = new Vector3Int(999, 999, 0);
        if (mousePos.x < 0 || mousePos.y > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl)) stopControls = true;
            PreviewArea.instance.gameObject.transform.localScale = new Vector3(0, 0, 0);
            StopActions();
            return;
        }
        
        if (MapProperties.instance.IsEmpty())
        {
            StopActions();
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (PreventPlaceBehindGUI.instance.behindUI) _clickedOnUI = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _clickedOnUI = false;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            StopActions();
            return;
        }

        if (PreventPlaceBehindGUI.instance.behindUI || _clickedOnUI)
        {
            StopActions();
            return;
        }

        if (!Input.GetKey(KeyCode.LeftControl))
        {
            if (!Input.GetKey(KeyCode.LeftShift)) startAreaPos = new Vector3Int();
        
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
            {
                startAreaPos = mousePos;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(1) && startAreaPos != new Vector3Int())
            {
                AreaPlaceTile(true);
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1))
            {
                PlaceTile(true);
            }
        
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(1)) PreviewArea.instance.AreaPreview(deleteColor, startAreaPos, mousePos);
        
            if (startAreaPos == new Vector3Int()) areaPreview.transform.localScale = new Vector3(0, 0, 0);
        }
        
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.LeftShift)) return;

            if (Input.GetMouseButtonDown(0) && startCopyPos == new Vector3Int())
            {
                startCopyPos = mousePos;
            }
            if (Input.GetMouseButtonUp(0) && endCopyPos == new Vector3Int())
            {
                endCopyPos = mousePos;
                CopyTiles();
                ObjectEditor.instance.CopyObjects(startCopyPos, endCopyPos);
            }
            if (Input.GetMouseButtonDown(0) && copiedIds.Count != 0 && startCopyPos != new Vector3Int() && endCopyPos != new Vector3Int())
            {
                PasteTiles();
            }
            if (copiedIds.Count != 0 && startCopyPos != new Vector3Int() && endCopyPos != new Vector3Int())
            {
                PreviewArea.instance.PastePreview(pasteColor, startCopyPos, endCopyPos);
            }
            if (Input.GetMouseButton(0) && startCopyPos != new Vector3Int() && copiedIds.Count == 0)
            {
                PreviewArea.instance.CopyPreview(copyColor, startCopyPos);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            startCopyPos = new();
            endCopyPos = new();
            copiedIds = new();
        }
        
        if (Input.GetMouseButtonDown(2) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt)))
        {
            var tile = currentTilemap[currentTilemapLayer].GetTile(mousePos) as CustomTile;
            if (tile != null)
            {
                selectedTileIndex = tile.id;
                placementMode = true;
            }
        }

        if (!placementMode) return;

        if (!Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
            {
                startAreaPos = mousePos;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(0) && startAreaPos != new Vector3Int())
            {
                AreaPlaceTile(false);
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
            {
                PlaceTile(false);
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)) PreviewArea.instance.AreaPreview(placeColor, startAreaPos);

        }
    }
    
    public void CopyTiles()
    {
        copiedIds.Clear();

        int xMin = Mathf.Min(startCopyPos.x, endCopyPos.x);
        int xMax = Mathf.Max(startCopyPos.x, endCopyPos.x);
        int yMin = Mathf.Min(startCopyPos.y, endCopyPos.y);
        int yMax = Mathf.Max(startCopyPos.y, endCopyPos.y);

        for (int y = yMin; y <= yMax; y++) {
            for (int x = xMin; x <= xMax; x++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                var tile = currentTilemap[currentTilemapLayer].GetTile(pos) as CustomTile;
                var tileId = tile ? tile.id : -1;
                copiedIds.Add(tileId);
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void PasteTiles()
    {
        var width = Mathf.Abs(endCopyPos.x - startCopyPos.x);
        var height = Mathf.Abs(endCopyPos.y - startCopyPos.y);

        var copyStartX = Mathf.Min(startCopyPos.x, endCopyPos.x);
        var copyStartY = Mathf.Min(startCopyPos.y, endCopyPos.y);
        
        int anchorX = endCopyPos.x - copyStartX;
        int anchorY = endCopyPos.y - copyStartY;
        
        var pasteStartX = mousePos.x - anchorX;
        var pasteStartY = mousePos.y - anchorY;

        var entries = new List<UndoEntry>();
        int count = 0;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                Vector3Int pos = new Vector3Int(pasteStartX + x, pasteStartY + y, 0);

                if (pos.x < 0 || pos.y > 0)
                {
                    count++;
                    continue;
                }

                var tile = currentTilemap[currentTilemapLayer].GetTile(pos) as CustomTile;
                var undoId = tile ? tile.id : -1;
                entries.Add(new UndoEntry()
                {
                    id = undoId,
                    isTile = true,
                    layer = currentTilemapLayer,
                    position = pos
                });

                var id = copiedIds[count] != -1 ? MapManager.instance.tiles[copiedIds[count]] : null;
                currentTilemap[currentTilemapLayer].SetTile(pos, id);

                count++;
            }
        }
        
        ObjectEditor.instance.AreaDeleteObject(new Vector3Int(pasteStartX, pasteStartY, 0), new Vector3Int(pasteStartX + width, pasteStartY + height, 0), entries, true);
    }
    
    private Vector2Int GetCopyDirection()
    {
        int xDir = endCopyPos.x >= startCopyPos.x ? 1 : -1;
        int yDir = endCopyPos.y >= startCopyPos.y ? 1 : -1;
        return new Vector2Int(xDir, yDir);
    }
    
    public List<int> copiedIds;
    public Vector3Int startCopyPos;
    public Vector3Int endCopyPos;

    public Vector3Int startAreaPos;
    private GameObject areaPreview => PreviewArea.instance.gameObject;
    
    private void AreaPlaceTile(bool delete)
    {
        var width = Mathf.Abs(mousePos.x - startAreaPos.x);
        var height = Mathf.Abs(mousePos.y - startAreaPos.y);

        var startX = Mathf.Min(startAreaPos.x, mousePos.x);
        var startY = Mathf.Min(startAreaPos.y, mousePos.y);
        
        var entries = new List<UndoEntry>();
        
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                Vector3Int pos = new Vector3Int(startX + x, startY + y, 0);
                
                // Undo shenanigans
                var tile = currentTilemap[currentTilemapLayer].GetTile(pos) as CustomTile;
                var undoId = tile ? tile.id : -1;
                var entry = new UndoEntry()
                {
                    id = undoId,
                    isTile = true,
                    layer = currentTilemapLayer,
                    position = pos
                };
                entries.Add(entry);
                
                if (delete) currentTilemap[currentTilemapLayer].SetTile(pos, null);
                else currentTilemap[currentTilemapLayer].SetTile(pos, CurrentTile);
            }
        }
        
        if (delete)
        {
            ObjectEditor.instance.AreaDeleteObject(startAreaPos, mousePos, entries);
        }
        else
        {
            UndoRedoManager.instance.SaveState(entries);
        }

        startAreaPos = new();
    }
    private void PlaceTile(bool delete)
    {
        var pos = mousePos;
        var tile = currentTilemap[currentTilemapLayer].GetTile(pos) as CustomTile;


        if (!tile && delete) return;
        if (tile && tile.id == selectedTileIndex && !delete)
        {
            return;
        }
        
        var entries = new List<UndoEntry>();
        // Undo shenanigans
        var undoId = tile ? tile.id : -1;
        var entry = new UndoEntry()
        {
            id = undoId,
            isTile = true,
            layer = currentTilemapLayer,
            position = pos
        };
        entries.Add(entry);
        UndoRedoManager.instance.SaveState(entries);

        if (delete)
        {
            if (pos != tempPos)
            {
                currentTilemap[currentTilemapLayer].SetTile(pos, null);
            }
        }
        else
        {
            currentTilemap[currentTilemapLayer].SetTile(pos, CurrentTile);
        }
    }

    public void ClearTiles()
    {
        foreach (Tilemap tilemap in currentTilemap)
        {
            tilemap.transform.parent.gameObject.SetActive(true);
            for (int x = 0; x < 200; x++)
            {
                for (int y = 0; y < 200; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, -y, 0), null);
                }
            }
            if (!tilemap.transform.parent.gameObject.name.Contains("Ground"))
            {
                tilemap.transform.parent.gameObject.SetActive(false);
            }
        }
        
        currentTilemap[1].gameObject.SetActive(true);
        MapManager.instance.currentLayer = 1;
    }
}

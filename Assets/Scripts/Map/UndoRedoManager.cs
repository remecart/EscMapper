using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class UndoRedoManager : MonoBehaviour
{
    public static UndoRedoManager instance;
    private readonly Stack<List<UndoEntry>> undoBatch = new();
    private readonly Stack<List<UndoEntry>> redoBatch = new();

    public void ClearEntries()
    {
        undoBatch.Clear();
        redoBatch.Clear();
    }

    private void Start()
    {
        instance = this;
    }

    public void Update()
    {
        if (!Input.GetKey(KeyCode.LeftControl)) return;
        
        if (Input.inputString.Contains("z")) Undo();
        if (Input.inputString.Contains("y")) Redo();
        //if (Input.GetKeyDown(KeyCode.Z)) Redo();
    }

    public void SaveState(List<UndoEntry> batch) // 
    {
        undoBatch.Push(batch);
        redoBatch.Clear();
    }

    public void Undo()
    {
        if (undoBatch.Count == 0) return;

        List<UndoEntry> batch = undoBatch.Pop();
        List<UndoEntry> redoGroup = new();

        foreach (var entry in batch)
        {
            if (entry.isTile)
            {
                var tilemap = TileEditor.instance.currentTilemap[entry.layer];
                var currentTile = tilemap.GetTile(Vector3Int.RoundToInt(entry.position)) as CustomTile;
                var currentId = currentTile ? MapManager.instance.tiles[currentTile.id].id : -1;

                redoGroup.Add(new UndoEntry
                {
                    isTile = true,
                    id = currentId,
                    layer = entry.layer,
                    position = entry.position
                });

                var newTile = entry.id == -1 ? null : MapManager.instance.tiles[entry.id];
                tilemap.SetTile(Vector3Int.RoundToInt(entry.position), newTile);
            }
            else
            {
                GameObject obj = FindObjectAtPosition(entry.position, entry.layer);

                if (obj != null)
                {
                    // Object exists — this was likely a placement, so we undo by destroying it
                    var co = obj.GetComponent<CustomObject>();
                    if (co != null)
                    {
                        redoGroup.Add(new UndoEntry {
                            isTile = false,
                            id = co.id,
                            layer = entry.layer,
                            position = obj.transform.position
                        });
                    }

                    Destroy(obj);
                }
                else
                {
                    // Object doesn't exist — this was a deletion, so we undo by restoring it
                    GameObject prefab = ObjectLookupTable.instance.objects[entry.id];
                    GameObject go = Instantiate(prefab, entry.position, Quaternion.identity, ObjectEditor.instance.ObjectLayers[entry.layer].transform);

                    var co = go.GetComponent<CustomObject>();
                    go.transform.position = entry.position;
                    go.GetComponent<SpriteRenderer>().sortingOrder = entry.layer * 50 + 25;

                    redoGroup.Add(new UndoEntry {
                        isTile = false,
                        id = entry.id,
                        layer = entry.layer,
                        position = entry.position
                    });
                }
            }
        }

        redoBatch.Push(redoGroup);
    }
    
    public void Redo()
    {
        if (redoBatch.Count == 0) return;

        List<UndoEntry> batch = redoBatch.Pop();
        List<UndoEntry> undoGroup = new();

        foreach (var entry in batch)
        {
            if (entry.isTile)
            {
                var tilemap = TileEditor.instance.currentTilemap[entry.layer];
                var currentTile = tilemap.GetTile(Vector3Int.RoundToInt(entry.position)) as CustomTile;
                var currentId = currentTile ? MapManager.instance.tiles[currentTile.id].id : -1;

                undoGroup.Add(new UndoEntry
                {
                    isTile = true,
                    id = currentId,
                    layer = entry.layer,
                    position = entry.position
                });

                var newTile = entry.id == -1 ? null : MapManager.instance.tiles[entry.id];
                tilemap.SetTile(Vector3Int.RoundToInt(entry.position), newTile);
            }
            else
            {
                GameObject prefab = ObjectLookupTable.instance.objects[entry.id];
                GameObject go = Instantiate(prefab, entry.position, Quaternion.identity, ObjectEditor.instance.ObjectLayers[entry.layer].transform);

                var co = go.GetComponent<CustomObject>();
                go.transform.position = entry.position; // exact world position
                go.GetComponent<SpriteRenderer>().sortingOrder = entry.layer * 50 + 25;

                undoGroup.Add(new UndoEntry {
                    isTile = false,
                    id = entry.id,
                    layer = entry.layer,
                    position = entry.position
                });
            }
        }

        undoBatch.Push(undoGroup);
    }

    private GameObject FindObjectAtPosition(Vector3 position, int layer)
    {
        Transform layerTransform = ObjectEditor.instance.ObjectLayers[layer].transform;
        foreach (Transform child in layerTransform)
        {
            if (Vector3.Distance(child.position, position) < 0.5f)
                return child.gameObject;
        }
        return null;
    }
}

[System.Serializable]
public class UndoEntry
{
    public bool isTile;
    public Vector3 position;
    public int id;
    public int layer;
}
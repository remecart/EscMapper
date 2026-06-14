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

    private float undoDelay = 0.5f;
    private float redoDelay = 0.5f;
    private float undoRepeatRate = 0.05f;
    private float redoRepeatRate = 0.05f;

    private float undoTimer = 0f;
    private float redoTimer = 0f;
    private bool undoHeld = false;
    private bool redoHeld = false;

    public void Update()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (ctrl)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                if (!undoHeld)
                {
                    Undo();
                    undoHeld = true;
                    undoTimer = undoDelay;
                }
                else
                {
                    undoTimer -= Time.deltaTime;
                    if (undoTimer <= 0f)
                    {
                        Undo();
                        undoTimer = undoRepeatRate;
                    }
                }
            }
            else undoHeld = false;

            if (Input.GetKey(KeyCode.Y))
            {
                if (!redoHeld)
                {
                    Redo();
                    redoHeld = true;
                    redoTimer = redoDelay;
                }
                else
                {
                    redoTimer -= Time.deltaTime;
                    if (redoTimer <= 0f)
                    {
                        Redo();
                        redoTimer = redoRepeatRate;
                    }
                }
            }
            else redoHeld = false;
        }
        else
        {
            undoHeld = false;
            redoHeld = false;
        }
    }

    public void SaveState(List<UndoEntry> batch)
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
                ApplyObjectState(entry, redoGroup);
            }
        }
        
        ShadowManager.instance.ReloadAllShadows();

        redoBatch.Push(redoGroup);
        SoundManager.instance.PlaySound("Undo");
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
                ApplyObjectState(entry, undoGroup);
            }
        }
        
        ShadowManager.instance.ReloadAllShadows();

        undoBatch.Push(undoGroup);
        SoundManager.instance.PlaySound("Redo");
    }

    private void ApplyObjectState(UndoEntry entry, List<UndoEntry> oppositeGroup)
    {
        GameObject existing = FindObjectAtPosition(entry.position, entry.layer);

        oppositeGroup.Add(new UndoEntry
        {
            isTile = false,
            id = entry.id,
            layer = entry.layer,
            position = entry.position,
            objectExistsAfter = existing != null
        });

        if (entry.objectExistsAfter)
        {
            if (existing == null)
            {
                CreateObject(entry);
            }
        }
        else
        {
            if (existing != null)
            {
                Destroy(existing);
            }
        }
    }

    private void CreateObject(UndoEntry entry)
    {
        GameObject prefab = ObjectLookupTable.instance.objects[entry.id];

        GameObject go = Instantiate(
            prefab,
            entry.position,
            Quaternion.identity,
            ObjectEditor.instance.ObjectLayers[entry.layer].transform);

        go.transform.position = entry.position;

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = entry.layer * 50 + 25;
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

    public bool objectExistsAfter;
}
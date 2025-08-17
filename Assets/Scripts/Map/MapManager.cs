using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public int currentLayer;

    public List<CustomTile> tiles = new List<CustomTile>();
    public List<GameObject> Layers;

    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        ChangeLayer(0);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeLayer(-1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeLayer(1);
    }

    public void ChangeLayer(int change)
    {
        if (MapProperties.instance.IsEmpty())
        {
            return;
        }

        if (currentLayer != Mathf.Clamp(currentLayer + change, 0, 4))
        {
            currentLayer = Mathf.Clamp(currentLayer + change, 0, 4);

            foreach (var layer in Layers)
            {
                layer.gameObject.SetActive(false);
            }

            Layers[currentLayer].SetActive(true);
            if (currentLayer > 1) Layers[1].SetActive(true);

            text.text = Layers[currentLayer].name;
        }
    }

    public void ReloadCustomTiles()
    {
        for (int i = 0; i < TextureManagement.instance.loadedTiles.Count + 1; i++)
        {
            var tile = ScriptableObject.CreateInstance<CustomTile>();
            tile.id = i;
            tile.tile = TextureManagement.instance.ReturnTile(i);
            tiles.Add(tile);
        }

        // LoadLevel();
    }

    public void SaveLevel()
    {
        MapProperties.instance.properties.Tiles.Clear();
        MapProperties.instance.properties.Objects.Clear();

        for (var i = 0; i < TileEditor.instance.currentTilemap.Count; i++)
        {
            var tilemap = TileEditor.instance.currentTilemap[i];
            BoundsInt bounds = tilemap.cellBounds;

            for (var x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (var y = bounds.min.y; y < bounds.max.y; y++)
                {
                    var customTile = tilemap.GetTile(new Vector3Int(x, y, 0)) as CustomTile;
                    if (!customTile) continue;
                    var entry = new Tiles
                    {
                        Id = customTile.id,
                        Position = new Vector2Int(x, y),
                        Layer = i
                    };
                    MapProperties.instance.properties.Tiles.Add(entry);
                }
            }
        }

        for (var i = 0; i < ObjectEditor.instance.ObjectLayers.Count; i++)
        {
            foreach (Transform child in ObjectEditor.instance.ObjectLayers[i].transform)
            {
                var go = child.gameObject;
                var obj = go.GetComponent<CustomObject>();
                var entry = new Objects()
                {
                    Id = obj.id,
                    Position = new Vector3(go.transform.position.x - obj.offset.x - 0.5f,
                        -(go.transform.position.y - obj.offset.y - 0.5f)),
                    Layer = i
                };
                MapProperties.instance.properties.Objects.Add(entry);
            }
        }
    }

    public void LoadLevel()
    {
        if (MapProperties.instance.properties.Tiles != null)
        {
            foreach (var tile in MapProperties.instance.properties.Tiles)
            {
                if (tile.Layer < 0 || tile.Layer >= TileEditor.instance.currentTilemap.Count)
                {
                    Debug.LogWarning(
                        $"[WARNING] MapManager.cs - Invalid layer of value {tile.Layer} discovered => Tile not loaded!");
                    continue;
                }

                if (tile.Id < 0 || tile.Id >= tiles.Count)
                {
                    Debug.LogWarning(
                        $"[WARNING] MapManager.cs - Invalid ID of value {tile.Id} discovered => Tile not loaded!");
                    continue;
                }

                TileEditor.instance.currentTilemap[tile.Layer].SetTile((Vector3Int)tile.Position, tiles[tile.Id]);
            }
        }

        if (MapProperties.instance.properties.Objects != null)
        {
            foreach (var obj in MapProperties.instance.properties.Objects)
            {
                if (obj.Id < ObjectLookupTable.instance.objects.Count)
                {
                    GameObject go = Instantiate(ObjectLookupTable.instance.objects[obj.Id]);
                    go.transform.SetParent(ObjectEditor.instance.ObjectLayers[obj.Layer].transform);
                    var co = go.GetComponent<CustomObject>();
                    go.GetComponent<SpriteRenderer>().sortingOrder = obj.Layer * 50 + 25;
                    go.transform.position = new Vector3(obj.Position.x + co.offset.x + 0.5f,
                        -(obj.Position.y - co.offset.y - 0.5f), 0);
                }
            }
        }

        ZoneEditor.instance.LoadZones();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapManager)), System.Serializable]
public class LevelManagerInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (MapManager)target;

        if (GUILayout.Button("Reload Custom Tiles"))
        {
            script.ReloadCustomTiles();
        }

        if (GUILayout.Button("Save Prison"))
        {
            script.SaveLevel();
        }

        if (GUILayout.Button("Load Prison"))
        {
            script.LoadLevel();
        }
    }
}
#endif
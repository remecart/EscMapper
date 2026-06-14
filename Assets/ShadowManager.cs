using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class ShadowManager : MonoBehaviour
{
    public static ShadowManager instance;

    public bool renderShadows;

    public List<int> shadowType = new List<int>
    {
        0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 3, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 3, 2, 1,
        0, 4, 0, 0, 0, 0, 1, 1, 1, 2, 1, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 2, 3, 2, 3, 1, 1, 1, 1, 2, 3, 2, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        1, 1, 3, 3, 2, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };


    private List<int> shadowBlocks = new List<int>
    {
        1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0,
        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
    };
    

    public ShadowTile blocker;

    public void Start()
    {
        instance = this;
    }
    

    List<int> Layers = new List<int>{1};
    public List<Tilemap> ShadowMaps;
    public List<Tilemap> ShadowBlockers;

    public void ToggleShadows()
    {
        renderShadows = !renderShadows;

        foreach (var map in ShadowMaps)
        {
            map.gameObject.SetActive(renderShadows);
        }
        
        foreach (var map in ShadowBlockers)
        {
            map.gameObject.SetActive(renderShadows);
        }
        
        if (renderShadows == true) ReloadAllShadows();
    }
    
    public void UpdateShadowsAround(Vector3Int pos, int layer)
    {
        if (!renderShadows) return;
        
        var source = TileEditor.instance.currentTilemap[TileEditor.instance.currentTilemapLayer];
        var target = layer == 1 ? ShadowMaps[0] : ShadowMaps[1];
        var targetBlocker = layer == 1 ? ShadowBlockers[0] : ShadowBlockers[1];

        // Check the placed tile + all 8 neighbours
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                Vector3Int checkPos = new Vector3Int(pos.x + x, pos.y + y, pos.z);

                CustomTile tile = source.GetTile(checkPos) as CustomTile;

                if (tile == null)
                {
                    // No tile here — clear shadow and blocker
                    target.SetTile(checkPos, null);
                    targetBlocker.SetTile(checkPos, null);
                    continue;
                }

                target.SetTile(checkPos, MapManager.instance.shadows[shadowType[tile.id - 1]]);

                if (shadowBlocks[tile.id - 1] == 0)
                    targetBlocker.SetTile(checkPos, MapManager.instance.shadows[8]);
                else
                    targetBlocker.SetTile(checkPos, null);
            }
        }
    }
    
    public void ReloadAllShadows()
    {
        if (!renderShadows) return;
        
        foreach (var layer in Layers)
        {
            var source = TileEditor.instance.currentTilemap[TileEditor.instance.currentTilemapLayer];
            var target = layer == 1 ? ShadowMaps[0] : ShadowMaps[1];
            var targetBlocker = layer == 1 ? ShadowBlockers[0] : ShadowBlockers[1];
            
            BoundsInt bounds = source.cellBounds;

            target.ClearAllTiles();
            targetBlocker.ClearAllTiles();
            
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                CustomTile center = source.GetTile(pos) as CustomTile;
                if (center == null) continue;

                if (shadowType[center.id - 1] == 1) // shadow ID tile 1 (box)
                {
                    //make changes here
                }

                target.SetTile(pos, MapManager.instance.shadows[shadowType[center.id - 1]]);
                if (shadowBlocks[center.id - 1] == 0) targetBlocker.SetTile(pos, MapManager.instance.shadows[8]);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowManager : MonoBehaviour
{
    public List<Tilemap> shadowTilemaps;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) CloneTilemap(TileEditor.instance.currentTilemap[1], shadowTilemaps[0]);
    }

    void CloneTilemap(Tilemap source, Tilemap target)
    {
        target.ClearAllTiles();

        BoundsInt bounds = source.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            CustomTile tile = source.GetTile(pos) as CustomTile;
            if (tile == null) continue;
            
            target.SetTile(pos, MapManager.instance.shadows[tile.id]);
        }
    } 
}

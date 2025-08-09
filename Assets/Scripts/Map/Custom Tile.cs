using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New CustomTile", menuName = "LevelEditor/Tile")]
public class CustomTile : Tile
{
    public int id;
    public Texture2D tile;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var texture = TextureManagement.instance.ReturnTile(id);
        if (texture == null)
        {
            texture = TextureManagement.instance.missingTile;
        }

        tileData.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, texture.width);
    }
}

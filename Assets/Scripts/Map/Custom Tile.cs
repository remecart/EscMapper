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

        tileData.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, texture.width);
    }
}

[CreateAssetMenu(fileName = "New ShadowTile", menuName = "LevelEditor/Shadow")]
public class ShadowTile : Tile
{
    public int id;
    public Texture2D tile;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var texture = TextureManagement.instance.ReturnShadow(id);

        tileData.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0.5f), texture.width / 2);
    }
}

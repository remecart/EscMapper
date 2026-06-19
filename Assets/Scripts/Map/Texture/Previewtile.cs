using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Previewtile : MonoBehaviour
{
    public static Previewtile instance;
    public Camera cam;
    private SpriteRenderer spriteRenderer;

    // Cached state to avoid recreating sprites every frame
    private int _lastTileIndex = -1;
    private bool _lastPlacementMode = true;
    private int _lastObjectIndex = -1;
    private Texture2D _cachedObjectTexture; // owned by us, must be destroyed on replacement

    private void Start()
    {
        instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (PreventPlaceBehindGUI.instance.behindUI || MapManager.instance.currentLayer == 4)
        {
            transform.localScale = Vector3.zero;
            return;
        }
        
        UpdateTexture();
    }

    private void UpdateTexture()
    {
        transform.localScale = Vector3.one;
        var pos = TileEditor.instance.currentTilemap[MapManager.instance.currentLayer].WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));

        bool placementMode = TileEditor.instance.placementMode;

        if (placementMode)
        {
            if (TextureManagement.instance.loadedTiles.Count == 0) return;

            int tileIndex = TileEditor.instance.selectedTileIndex;

            // Only rebuild the sprite when the selected tile actually changes
            if (tileIndex != _lastTileIndex || !_lastPlacementMode)
            {
                DestroyCurrentSprite(destroyTexture: false); // tile textures are owned by TextureManagement
                var texture = TextureManagement.instance.ReturnTile(tileIndex);
                spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, texture.width);
                _lastTileIndex = tileIndex;
                _lastPlacementMode = true;
            }

            transform.position = pos;
        }
        else
        {
            if (ObjectLookupTable.instance.objects.Count == 0) return;

            int objectIndex = ObjectEditor.instance.selectedObjectIndex;

            // Only rebuild the sprite when the selected object actually changes
            if (objectIndex != _lastObjectIndex || _lastPlacementMode)
            {
                var co = ObjectLookupTable.instance.objects[objectIndex].GetComponent<CustomObject>();
                var texture = TextureManagement.instance.ReturnObject(TileEditor.instance.selectedTileIndex, co.rect);

                DestroyCurrentSprite(destroyTexture: true); // object textures are created by ReturnObject, we own them
                spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 16);
                _cachedObjectTexture = texture;
                _lastObjectIndex = objectIndex;
                _lastPlacementMode = false;
            }

            var selectedCo = ObjectLookupTable.instance.objects[objectIndex].GetComponent<CustomObject>();
            float.TryParse(ObjectEditor.instance.OffsetX.text, out float parsedX);
            float.TryParse(ObjectEditor.instance.OffsetY.text, out float parsedY);
            transform.position = new Vector3(pos.x + selectedCo.offset.x + 0.5f + parsedX, pos.y + selectedCo.offset.y + 0.5f + parsedY, 0);
        }
    }

    private void DestroyCurrentSprite(bool destroyTexture)
    {
        if (spriteRenderer.sprite != null)
        {
            Destroy(spriteRenderer.sprite);
            spriteRenderer.sprite = null;
        }

        if (destroyTexture && _cachedObjectTexture != null)
        {
            Destroy(_cachedObjectTexture);
            _cachedObjectTexture = null;
        }
    }

    private void OnDestroy()
    {
        DestroyCurrentSprite(destroyTexture: true);
    }
}
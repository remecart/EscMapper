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
        
        if (TileEditor.instance.placementMode)
        {
            if (TextureManagement.instance.loadedTiles.Count == 0)
            {
                return;
            }
        
            var texture = TextureManagement.instance.ReturnTile(TileEditor.instance.selectedTileIndex);
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, texture.width);
            
            transform.position = pos;
        }
        else
        {
            if (ObjectLookupTable.instance.objects.Count == 0)
            {
                return;
            }
            
            var co = ObjectLookupTable.instance.objects[ObjectEditor.instance.selectedObjectIndex].GetComponent<CustomObject>();
            var texture = TextureManagement.instance.ReturnObject(TileEditor.instance.selectedTileIndex, co.rect);
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 16);

            float parsedX, parsedY;

            float.TryParse(ObjectEditor.instance.OffsetX.text, out parsedX);
            float.TryParse(ObjectEditor.instance.OffsetY.text, out parsedY);
            
            transform.position = new Vector3(pos.x + co.offset.x + 0.5f + parsedX, pos.y + co.offset.y + 0.5f + parsedY, 0);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewArea : MonoBehaviour
{
    public Camera cam => transform.parent.GetComponent<Camera>();
    public Vector3Int mousePos => TileEditor.instance.currentTilemap[MapManager.instance.currentLayer].WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
    public SpriteRenderer sprite => GetComponent<SpriteRenderer>();
    public static PreviewArea instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    
    public void AreaPreview(Color color, Vector3Int startAreaPos, Vector3Int endAreaPos = new Vector3Int())
    {
        if (endAreaPos == new Vector3Int()) endAreaPos = mousePos;
        
        float width = Mathf.Abs(endAreaPos.x - startAreaPos.x) + 1;
        float height = Mathf.Abs(endAreaPos.y - startAreaPos.y) + 1;

        float xPos = ((float)startAreaPos.x + (float)endAreaPos.x) / 2 + 0.5f;
        float yPos = ((float)startAreaPos.y + (float)endAreaPos.y) / 2 + 0.5f;

        transform.position = new Vector3(xPos, yPos, 0);
        transform.localScale = new Vector3(width, height, 1);
        GetComponent<SpriteRenderer>().color = color;
    }
    
    public void CopyPreview(Color color, Vector3Int startCopyPos)
    {
        if (startCopyPos == new Vector3Int()) return;

        var currentMousePos = mousePos;

        int xMin = Mathf.Min(startCopyPos.x, currentMousePos.x);
        int xMax = Mathf.Max(startCopyPos.x, currentMousePos.x);
        int yMin = Mathf.Min(startCopyPos.y, currentMousePos.y);
        int yMax = Mathf.Max(startCopyPos.y, currentMousePos.y);

        float previewWidth = xMax - xMin + 1;
        float previewHeight = yMax - yMin + 1;

        float xPos = xMin + previewWidth / 2f;
        float yPos = yMin + previewHeight / 2f;

        transform.position = new Vector3(xPos, yPos, 0);
        transform.localScale = new Vector3(previewWidth, previewHeight, 1);
        GetComponent<SpriteRenderer>().color = color;
    }

    public void PastePreview(Color color, Vector3Int startCopyPos, Vector3Int endCopyPos)
    {
        var width = Mathf.Abs(endCopyPos.x - startCopyPos.x);
        var height = Mathf.Abs(endCopyPos.y - startCopyPos.y);

        var copyStartX = Mathf.Min(startCopyPos.x, endCopyPos.x);
        var copyStartY = Mathf.Min(startCopyPos.y, endCopyPos.y);

        var currentMousePos = mousePos;

        int anchorX = endCopyPos.x - copyStartX;
        int anchorY = endCopyPos.y - copyStartY;

        float previewStartX = currentMousePos.x - anchorX;
        float previewStartY = currentMousePos.y - anchorY;

        float previewWidth = width + 1;
        float previewHeight = height + 1;

        float xPos = previewStartX + previewWidth / 2f;
        float yPos = previewStartY + previewHeight / 2f;

        transform.position = new Vector3(xPos, yPos, 0);
        transform.localScale = new Vector3(previewWidth, previewHeight, 1);
        sprite.color = color;
    }
}

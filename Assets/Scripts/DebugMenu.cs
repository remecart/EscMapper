using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DebugMenu : MonoBehaviour
{
    public List<string> lines;
    public Vector2Int mousePos => (Vector2Int)TileEditor.instance.mousePos;

    public List<TextMeshProUGUI> textElements;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(UpdateDebug), 0.1f, 0.1f);
    }

    // Update is called once per frame
    void UpdateDebug()
    {
        lines.Clear();
        lines.Add($"EscMapper // v1.2.0");
        lines.Add($" FPS: {(1f / Time.deltaTime).ToString("F2", CultureInfo.InvariantCulture)}");
        lines.Add($" Selected tile ID: {TileEditor.instance.selectedTileIndex} ({TileProperties.instance.currentProperties[TileEditor.instance.selectedTileIndex - 1]})");
        lines.Add($" Selected object ID: {ObjectEditor.instance.selectedObjectIndex} ({ObjectLookupTable.instance.objects[ObjectEditor.instance.selectedObjectIndex].name.Substring(ObjectLookupTable.instance.objects[ObjectEditor.instance.selectedObjectIndex].name.IndexOf("]") + 1).Trim().Replace("(Clone)", "")})");
        string mode = TileEditor.instance.placementMode == true ? "Tile" : "Object";
        lines.Add($" Placement mode: {mode}");
        lines.Add($" Mouse Position: {mousePos.x} | {mousePos.y * -1f}");

        DisplayText();
    }

    void DisplayText() {
        for (int i = 0; i < lines.Count; i++) {
            textElements[i].text = lines[i];
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExtendTileset : MonoBehaviour
{
    public SmoothScrollRectTransform smooth;
    public static ExtendTileset instance;
    public GameObject extraTiles;
    public Transform content;
    public Texture2D tex;

    void Start()
    {
        instance = this;
    }

    public void Extend(bool extension, int amount = 0)
    {
        var rect = content.GetComponent<RectTransform>();

        for (int i = content.childCount - 1; i >= 100; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        extraTiles.gameObject.GetComponent<RawImage>().texture = tex;

        if (extension)
        {
            for (int x = 0; x <= amount / 2 - 1; x++)
            {
                // Blank tile
                CreateTile(9999, "BLANK");

                // Actual tile
                CreateTile(100 + x * 2 + 1, (100 + x * 2 + 1).ToString());

                // Blank tile
                CreateTile(9999, "BLANK");

                // Actual tile
                CreateTile(100 + x * 2, (100 + x * 2).ToString());
            }

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)(30 * (25 + (float)amount / 2) * 0.755));

            smooth.enabled = true;
        }
        else
        {
            smooth.enabled = false;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, (float)(30 * 25 * 0.755));
        }
    }

    private void CreateTile(int value, string name)
    {
        var go = Instantiate(extraTiles, content, true);
        go.name = name;

        var trigger = go.GetComponent<EventTrigger>();
        AddIntCallback(trigger, EventTriggerType.PointerDown, value);
    }

    // Build-safe, runtime-only event trigger setup
    public void AddIntCallback(EventTrigger trigger, EventTriggerType type, int newValue)
    {
        // Remove ALL persistent and runtime entries
        trigger.triggers.Clear();

        // Create new entry
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;

        // Add runtime callback with your int argument
        entry.callback.AddListener((eventData) =>
        {
            TileClicked(newValue);
        });

        trigger.triggers.Add(entry);
    }

    private void TileClicked(int value)
    {
        TileEditor.instance.ChangeSelectedTile(value);
    }
}
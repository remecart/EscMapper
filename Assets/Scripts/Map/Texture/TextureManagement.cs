using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using ThreeDISevenZeroR.UnityGifDecoder;
using TMPro;
using UnityEngine.Tilemaps;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class TextureManagement : MonoBehaviour
{
    public List<Texture2D> loadedTiles;
    public static TextureManagement instance;
    public Texture2D missingTile;
    public SpriteRenderer groundTexture;
    public SpriteRenderer undergroundTexture;
    public Texture2D objectTexture;

    public TMP_Dropdown grounds;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    
    public void GroundHelper()
    {
        int tileSize = 16;
        int width = 108;
        int height = 108;

        Texture2D finalTexture = new Texture2D(width * tileSize, height * tileSize, TextureFormat.RGBA32, false);

        // Transparent pixel array
        Color[] clearPixels = Enumerable.Repeat(new Color(0, 0, 0, 0), tileSize * tileSize).ToArray();
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = new Color32(0, 0, 0, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, -107 + y, 0);
                CustomTile tile = TileEditor.instance.currentTilemap[1].GetTile(tilePos) as CustomTile;

                int px = x * tileSize;
                int py = y * tileSize;


                if (tile != null)
                {
                    Texture2D tileTexture = loadedTiles[tile.id - 1];
                    Color[] pixels = tileTexture.GetPixels();
                    finalTexture.SetPixels(px, py, tileSize, tileSize, pixels);
                }
                else
                {
                    finalTexture.SetPixels(px, py, tileSize, tileSize, clearPixels);
                }
            }
        }

        finalTexture.Apply();

        // Save PNG
        string fileName = "GroundHelper.png";
        string filePath = Path.Combine(FolderPath.instance.Config.sourceFolderPath, "Data", "images", "custom", fileName);
        File.WriteAllBytes(filePath, finalTexture.EncodeToPNG());
    }
    

    private void GetCusTextures()
    {
        // TE1 actually loads whatever textures you want as long as you follow the naming scheme ground_cus_<mapName> so here is me adding support for infinite ground layers
        // The same technically works with tilesets but ingame the tiles will not have any collisions which is why I didn't implement it
        
        var path = Path.Combine(FolderPath.instance.Config.sourceFolderPath, "Data", "images", "custom");
        string[] files = Directory.GetFiles(path);
        List<string> validFiles = new();

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            if (fileName.Contains("ground_cus"))
            {
                validFiles.Add(fileName.Replace("ground_cus_", "").Replace(".gif", ""));
            }
        }
        
        List<TMP_Dropdown.OptionData> newOptions = validFiles
            .Select(name => new TMP_Dropdown.OptionData(name))
            .ToList();
        
        bool isDifferent = grounds.options.Count != newOptions.Count ||
                           !grounds.options.Select(o => o.text).SequenceEqual(validFiles);

        if (isDifferent)
        {
            grounds.ClearOptions();
            grounds.options = newOptions;
        }
        
        string currentFloor = MapProperties.instance.properties.Info.Floor;
        int index = validFiles.IndexOf(currentFloor);
        if (index >= 0)
        {
            grounds.value = index;
        }
        else
        {
            grounds.value = 0;
        }

        grounds.RefreshShownValue();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ReloadTextures()
    {
        var path = Path.Combine(FolderPath.instance.Config.sourceFolderPath, "Data", "images", "custom");
        GetCusTextures();
        StartCoroutine(LoadTileset(MapProperties.instance.properties.Info.Tileset, path));
        StartCoroutine(LoadGround(MapProperties.instance.properties.Info.Floor, path));
        StartCoroutine(LoadUnderground(path));

        ReloadObjectTextures.instance.ReloadTextures();
    }

    private IEnumerator LoadGround(string tileset, string path)
    {
        var file = $"ground_cus_{tileset}.gif";
        var fullPath = Path.Combine(path, file);

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"[WARNING] TextureManagement.cs - {file} does not exist!");
            yield break;
        }

        var texture = LoadFirstGifFrame(fullPath);
        
        var cropped = CropGroundTexture(texture, 1728, 1728);
        cropped.filterMode = FilterMode.Point;
        groundTexture.sprite = Sprite.Create(cropped, new Rect(0, 0, cropped.width, cropped.height),
            new Vector2(0, 1), 16);
    }

    private IEnumerator LoadUnderground(string path)
    {
        var file = $"soil_cus.gif";
        var fullPath = Path.Combine(path, file);

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"[WARNING] TextureManagement.cs - soil_cus.gif does not exist!");
            yield break;
        }

        var texture = LoadFirstGifFrame(fullPath);

        var cropped = CropGroundTexture(texture, 1728, 1728);
        cropped.filterMode = FilterMode.Point;
        undergroundTexture.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0, 1), 16);
    }

    public Texture2D CropGroundTexture(Texture2D sourceTexture, int cropWidth, int cropHeight)
    {
        // Ensure the crop area fits within the source texture
        if (sourceTexture.width < cropWidth || sourceTexture.height < cropHeight)
        {
            Debug.LogWarning(
                $"[WARNING] TextureManagement.cs - Source texture too small to crop ({sourceTexture.width}x{sourceTexture.height} < {cropWidth}x{cropHeight})");
            return sourceTexture; // Or return missingTile if you prefer a placeholder
        }

        // Crop from top-left corner (Unity's origin is bottom-left, so we offset Y)
        int startX = 0;
        int startY = sourceTexture.height - cropHeight;

        Color[] pixels = sourceTexture.GetPixels(startX, startY, cropWidth, cropHeight);

        var croppedTexture = new Texture2D(cropWidth, cropHeight, sourceTexture.format, false);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        return croppedTexture;
    }

    Texture2D LoadFirstGifFrame(string path)
    {
        using (var stream = new GifStream(File.OpenRead(path)))
        {
            while (stream.HasMoreData)
            {
                if (stream.CurrentToken == GifStream.Token.Image)
                {
                    var image = stream.ReadImage();
                    Texture2D tex = new Texture2D(
                        stream.Header.width,
                        stream.Header.height,
                        TextureFormat.ARGB32,
                        false
                    );
                    tex.SetPixels32(image.colors);
                    tex.Apply();
                    return tex;
                }

                stream.SkipToken();
            }
        }

        return null;
    }


    private IEnumerator LoadTileset(string tileset, string path)
    {
        loadedTiles.Clear();
        var file = $"tiles_cus_{tileset}.gif";
        var fullPath = Path.Combine(path, file);

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"[WARNING] TextureManagement.cs - {file} does not exist!");
            yield break;
        }

        byte[] gifBytes = File.ReadAllBytes(fullPath);

        var texture = LoadFirstGifFrame(fullPath);
        SpitTextureToTiles(texture);

        foreach (var tilemap in TileEditor.instance.currentTilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    CustomTile tile = tilemap.GetTile(pos) as CustomTile;

                    if (tile is CustomTile)
                    {
                        tilemap.RefreshTile(pos);
                    }
                }
            }
        }
    }

    // Split all the tilesets up into individual tile textures
    private void SpitTextureToTiles(Texture2D texture)
    {
        texture.EncodeToPNG();
        var tileRes = texture.width / 4 == texture.height / 25 ? texture.width / 4 : 0;

        if (tileRes == 0)
        {
            Debug.LogWarning(
                $"[WARNING] TextureManagement.cs - Resolution must match an aspect ratio of 4:25 and be dividable by 16! => Tileset not loaded!");
            return;
        }

        // Inverted x/y forlooping for splitting the texture because TE1 does it :/

        for (var x = 0; x < 4; x++)
        {
            for (var y = 0; y < 25; y++)
            {
                var rect = new Rect(x * tileRes, texture.height - (y + 1) * tileRes, tileRes, tileRes);
                var pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].r == 1 && pixels[i].g == 1 && pixels[i].b == 1)
                    {
                        var color = pixels[i];
                        color.a = 0;
                        pixels[i] = color;
                    }
                    else if (pixels[i].r == 0 && pixels[i].g == 0 && pixels[i].b == 0)
                    {
                        var color = pixels[i];
                        color.a = 1;
                        pixels[i] = color;
                    }
                }

                var tile = new Texture2D(tileRes, tileRes);
                tile.filterMode = FilterMode.Point;
                tile.SetPixels(pixels);
                tile.Apply();

                loadedTiles.Add(tile);
            }
        }

        MapManager.instance.ReloadCustomTiles();
        TileSelection.instance.ReloadPageTextures();
    }


    public Texture2D ReturnTile(int id)
    {
        if (loadedTiles.Count == 0 || id == 0)
        {
            return missingTile;
        }

        if (loadedTiles[id - 1]) return loadedTiles[id - 1];
        return missingTile;
    }

    // Code for loading a seperate object texture, although its probably not needed
    public void LoadObjectTexture()
    {
        var path = ""; // FolderPath.instance.path + "\\textures\\objects.png";
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[DEBUG] TileTextureManagement.cs - {{objects.png}} could not be found!");
            return;
        }

        var bytes = File.ReadAllBytes(path);
        var texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        texture.EncodeToPNG();
        objectTexture = texture;
    }

    public Texture2D ReturnObject(int id, Rect rect = new Rect())
    {
        var pixels = objectTexture.GetPixels((int)rect.x * 16, (int)rect.y * 16, (int)rect.width * 16,
            (int)rect.height * 16);
        var tex = new Texture2D((int)rect.width * 16, (int)rect.height * 16);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixels);
        tex.Apply();

        return tex;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextureManagement)), System.Serializable]
public class TileTextureInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TextureManagement script = (TextureManagement)target;

        if (GUILayout.Button("Reload Textures"))
        {
            script.ReloadTextures();
        }
    }
}
#endif
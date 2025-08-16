using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using EscapistsMapTools.Encryption;
using SFB;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class MapProperties : MonoBehaviour
{
    public static MapProperties instance;

    public Properties properties;
    public Properties defaultProperties;

    public List<TMPro.TMP_Dropdown> dropdowns;
    public List<TMPro.TMP_InputField> inputfields;
    public List<Toggle> toggles;
    public bool loaded;
    public TMP_InputField mapName;
    public GameObject NewMapUI;
    
    public bool IsEmpty()
    {
        return !loaded;
    }

    private void Start()
    {
        instance = this;

        properties = new Properties();
    }

    public void PropertiesUI()
    {
        var dropdownSetters = new Action<int>[]
        {
            index => properties.Info.RoutineSet = dropdowns[0].options[index].text,
            index => properties.Info.NPClvl = index + 1,
            index => properties.Info.Music = dropdowns[2].options[index].text,
            index =>
            {
                properties.Info.Floor = dropdowns[3].options[index].text;
                TextureManagement.instance.ReloadTextures();
            },
            index => properties.Info.MapType = dropdowns[4].options[index].text,
            index =>
            {
                properties.Info.Tileset = dropdowns[5].options[index].text;
                TextureManagement.instance.ReloadTextures();
            },
            index => properties.Info.Inmates = index,
            index => properties.Info.Guards = index,
            index => properties.Jobs.StartingJob = dropdowns[8].options[index].text
        };

        for (int i = 0; i < dropdowns.Count && i < dropdownSetters.Length; i++)
        {
            int index = i; // capture index to avoid closure issues
            dropdowns[index].onValueChanged.AddListener(value => { dropdownSetters[index](value); });
        }

        var inputSetters = new Action<string>[]
        {
            value => properties.Info.MapName = value,
            value => properties.Info.Warden = value,
            value => properties.Info.Intro = value
        };

        for (int i = 0; i < inputfields.Count && i < inputSetters.Length; i++)
        {
            int index = i;
            inputfields[index].onValueChanged.AddListener(value => { inputSetters[index](value); });
        }

        var jobSetters = new Action<bool>[]
        {
            isOn => properties.Jobs.Janitor = isOn,
            isOn => properties.Jobs.Gardening = isOn,
            isOn => properties.Jobs.Tailor = isOn,
            isOn => properties.Jobs.Laundry = isOn,
            isOn => properties.Jobs.Woodshop = isOn,
            isOn => properties.Jobs.Library = isOn,
            isOn => properties.Jobs.Deliveries = isOn,
            isOn => properties.Jobs.Mailman = isOn,
            isOn => properties.Jobs.Kitchen = isOn,
            isOn => properties.Jobs.Metalshop = isOn
        };

        for (int i = 0; i < toggles.Count && i < jobSetters.Length; i++)
        {
            int index = i;
            toggles[index].onValueChanged.AddListener(isOn => { jobSetters[index](isOn); });
        }
    }


    private void LoadUIFromProperties()
    {
        // Dropdowns
        dropdowns[0].value = dropdowns[0].options.FindIndex(opt => opt.text == properties.Info.RoutineSet);
        dropdowns[1].value = Mathf.Clamp(properties.Info.NPClvl - 1, 0, dropdowns[1].options.Count - 1);
        dropdowns[2].value = dropdowns[2].options.FindIndex(opt => opt.text == properties.Info.Music);
        dropdowns[3].value = dropdowns[3].options.FindIndex(opt => opt.text == properties.Info.Floor);
        dropdowns[4].value = dropdowns[4].options.FindIndex(opt => opt.text == properties.Info.MapType);
        dropdowns[5].value = dropdowns[5].options.FindIndex(opt => opt.text == properties.Info.Tileset);
        dropdowns[6].value = Mathf.Clamp(properties.Info.Inmates, 0, dropdowns[6].options.Count - 1);
        dropdowns[7].value = Mathf.Clamp(properties.Info.Guards, 0, dropdowns[7].options.Count - 1);

        // InputFields
        inputfields[0].text = properties.Info.MapName;
        inputfields[1].text = properties.Info.Warden;
        inputfields[2].text = properties.Info.Intro;

        // Toggles
        toggles[0].isOn = properties.Jobs.Janitor;
        toggles[1].isOn = properties.Jobs.Gardening;
        toggles[2].isOn = properties.Jobs.Tailor;
        toggles[3].isOn = properties.Jobs.Laundry;
        toggles[4].isOn = properties.Jobs.Woodshop;
        toggles[5].isOn = properties.Jobs.Library;
        toggles[6].isOn = properties.Jobs.Deliveries;
        toggles[7].isOn = properties.Jobs.Mailman;
        toggles[8].isOn = properties.Jobs.Kitchen;
        toggles[9].isOn = properties.Jobs.Metalshop;
    }

    public void CreateNewMap()
    {
        properties = new Properties();
        properties = defaultProperties.Clone();
        properties.Info.MapName = mapName.text;
        properties.Info.Floor = "perks";
        
        foreach (GameObject layer in ObjectEditor.instance.ObjectLayers)
        {
            foreach (Transform child in layer.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        UndoRedoManager.instance.ClearEntries();
        LoadUIFromProperties();
        TileEditor.instance.ClearTiles();
        TextureManagement.instance.ReloadTextures();
        PropertiesUI();
        PerimeterVisualizer.instance.Visualize();
        
        
        loaded = true;
    }

    public void ToggleNewMapUI()
    {
        NewMapUI.SetActive(!NewMapUI.activeSelf);
    }


    public void LoadMap()
    {
        var filters = new[]
        {
            new ExtensionFilter("Project Files", "proj"),
            new ExtensionFilter("Custom Map Files", "cmap"),
            new ExtensionFilter("Official Map Files", "map")
        };
        
        var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "The Escapists", "Custom Maps");
        if (!Directory.Exists(documentsPath)) documentsPath = "";
        
        var path = StandaloneFileBrowser.OpenFilePanel("Select File", documentsPath, filters, false)[0];
        if (!File.Exists(path)) return;
        
        properties = new Properties();

        TileEditor.instance.ClearTiles();
        
        foreach (GameObject layer in ObjectEditor.instance.ObjectLayers)
        {
            foreach (Transform child in layer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        UndoRedoManager.instance.ClearEntries();
        Parser.LoadPrisonData(path, properties);
        LoadUIFromProperties();
        TextureManagement.instance.ReloadTextures();
        MapManager.instance.LoadLevel();
        PropertiesUI();
        PerimeterVisualizer.instance.Visualize();
        loaded = true;
    }

    public void SaveMap()
    {
        var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "The Escapists", "Custom Maps");
        if (!Directory.Exists(documentsPath)) documentsPath = "";
        var path = StandaloneFileBrowser.SaveFilePanel("Save Project", documentsPath, properties.Info.MapName, "proj");
        
        properties.Info.Custom = -1;
        properties.Info.Rdy = 0;
        properties.Info.Version = "";
        Parser.SavePrisonProperties(path, properties);
    }
    
    public void ExportMap()
    {
        var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "The Escapists", "Custom Maps");
        if (!Directory.Exists(documentsPath)) documentsPath = "";
        var path = StandaloneFileBrowser.SaveFilePanel("Save Project", documentsPath, properties.Info.MapName, "cmap");
        
        properties.Info.Custom = 2;
        properties.Info.Rdy = 1;
        properties.Info.Version = GenerateHexKey();
        Parser.SavePrisonProperties(path, properties);
    }
    
    public static string GenerateHexKey()
    {
        byte[] buffer = new byte[16]; // 16 bytes = 32 hex characters
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }

        StringBuilder sb = new StringBuilder(32);
        foreach (byte b in buffer)
        {
            sb.Append(b.ToString("x2")); // Convert to lowercase hex
        }

        return sb.ToString();
    }

}


//
// The only reason why this abomination of code exists is because of the file formatting in TE1 being very ass 

public class Parser
{
    public static void SavePrisonProperties(string filePath, Properties properties)
    {
        StringBuilder sb = new();

        // Jobs
        if (properties.Jobs != null)
        {
            sb.AppendLine("[Jobs]");
            var fields = typeof(Jobs).GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(bool))
                {
                    bool value = (bool)field.GetValue(properties.Jobs);
                    sb.AppendLine($"{field.Name}={(value ? 1 : 0)}");
                }
                else if (field.FieldType == typeof(string))
                {
                    string value = (string)field.GetValue(properties.Jobs);
                    if (!string.IsNullOrEmpty(value))
                        sb.AppendLine($"{field.Name}={value}");
                }
            }

            sb.AppendLine();
        }

        // Info
        if (properties.Info != null)
        {
            sb.AppendLine("[Info]");
            var fields = typeof(Info).GetFields();
            foreach (var field in fields)
            {
                object value = field.GetValue(properties.Info);
                if (value != null)
                    sb.AppendLine($"{field.Name}={value}");
            }

            sb.AppendLine();
        }

        // Zones
        if (properties.Zones != null)
        {
            sb.AppendLine("[Zones]");
            var fields = typeof(Zones).GetFields();
            foreach (var field in fields)
            {
                var list = field.GetValue(properties.Zones) as List<Zone>;
                if (list == null || list.Count == 0) continue;

                foreach (var zone in list)
                {
                    string line = $"{field.Name}={zone.startPos.x}_{zone.startPos.y}_{zone.endPos.x}_{zone.endPos.y}";
                    sb.AppendLine(line);
                }
            }

            sb.AppendLine();
        }

        if (properties.Perim != null)
        {
            sb.AppendLine("[Perim]");
            var fields = typeof(Perim).GetFields();

            foreach (var field in fields)
            {
                var value = (int)field.GetValue(properties.Perim);

                // Reverse the transformation
                if (field.Name == "Bottom" || field.Name == "Right")
                {
                    value = 107 - value;
                }

                int final = value * 16;

                sb.AppendLine($"{field.Name}={final}");
            }

            sb.AppendLine();
        }

        // Objects
        if (properties.Objects != null && properties.Objects.Count > 0)
        {
            sb.AppendLine("[Objects]");
            for (int i = 0; i < properties.Objects.Count; i++)
            {
                var obj = properties.Objects[i];
                var layer = obj.Layer == 0 ? 4 : obj.Layer;
                sb.AppendLine($"{i + 1}={obj.Position.x.ToString(CultureInfo.InvariantCulture)}x{obj.Position.y.ToString(CultureInfo.InvariantCulture)}x{obj.Id}x{layer}");
            }

            sb.AppendLine();
        }

        // Tiles
        Dictionary<int, Dictionary<int, string[]>> tileData = new(); // Layer -> Y -> X list

        foreach (var tile in properties.Tiles)
        {
            var tileLayer = tile.Layer;
            tileLayer = tileLayer == 0 ? 4 : tileLayer;
            tileLayer -= 1;

            if (!tileData.ContainsKey(tileLayer))
                tileData[tileLayer] = new();

            var layerDict = tileData[tileLayer];

            if (!layerDict.ContainsKey(tile.Position.y))
                layerDict[tile.Position.y] = new string[512]; // assume map width max

            layerDict[tile.Position.y][tile.Position.x] = tile.Id.ToString();
        }

        string[] tileHeaders = { "Tiles", "Vents", "Roof", "Underground" };
        for (int layer = 0; layer < tileHeaders.Length; layer++)
        {
            if (!tileData.ContainsKey(layer)) continue;

            sb.AppendLine($"[{tileHeaders[layer]}]");
            foreach (var kvp in tileData[layer])
            {
                int y = -kvp.Key;
                string[] xRow = kvp.Value;
                string row = "";
                for (int x = 0; x < xRow.Length; x++)
                {
                    row += (xRow[x] ?? "0") + "_";
                }

                sb.AppendLine($"{y}={row}");
            }

            sb.AppendLine();
        }

        File.WriteAllText(filePath, sb.ToString());
    }

    public static void LoadPrisonData(string filePath, Properties properties)
    {
        string[] lines = null;
        
        if (Path.GetExtension(filePath).Equals(".map", StringComparison.OrdinalIgnoreCase))
        {
            byte[] fileBytes;
            string key = "mothking";

            fileBytes = File.ReadAllBytes(filePath);
            BlowfishCompat decryptionBlowfish = new BlowfishCompat(key);
            byte[] decryptedData = decryptionBlowfish.Decrypt(fileBytes);
            string decryptedString = Encoding.ASCII.GetString(decryptedData);
            
            lines = decryptedString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }
        else
        {
            lines = File.ReadAllLines(filePath);
        }

        object currentSectionObject = null;
        Type currentSectionType = null;
        string currentSectionName = null;

        // For tile layers
        Dictionary<string, int> tileLayers = new()
        {
            { "Tiles", 0 },
            { "Vents", 1 },
            { "Roof", 2 },
            { "Underground", 3 },
        };

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                currentSectionName = trimmed.Trim('[', ']');
                currentSectionObject = null;
                currentSectionType = null;

                if (currentSectionName == "Zones")
                {
                    if (properties.Zones == null) properties.Zones = new Zones();
                    currentSectionObject = properties.Zones;
                    currentSectionType = typeof(Zones);
                }
                else if (currentSectionName == "Jobs")
                {
                    if (properties.Jobs == null) properties.Jobs = new Jobs();
                    currentSectionObject = properties.Jobs;
                    currentSectionType = typeof(Jobs);
                }
                else if (currentSectionName == "Info")
                {
                    if (properties.Info == null) properties.Info = new Info();
                    currentSectionObject = properties.Info;
                    currentSectionType = typeof(Info);
                }
                else if (currentSectionName == "Perim")
                {
                    if (properties.Perim == null) properties.Perim = new Perim();
                    currentSectionObject = properties.Perim;
                    currentSectionType = typeof(Perim);
                }

                continue;
            }

            if (string.IsNullOrWhiteSpace(currentSectionName))
                continue;

            // ZONES
            if (currentSectionName == "Zones")
            {
                var parts = trimmed.Split('=');
                if (parts.Length != 2) continue;

                var field = currentSectionType.GetField(parts[0]);
                if (field == null) continue;

                var values = parts[1].Split('_');
                if (values.Length != 4) continue;

                float px = float.Parse(values[0]);
                float py = float.Parse(values[1]);
                float sx = float.Parse(values[2]);
                float sy = float.Parse(values[3]);

                var zoneList = new List<Zone>
                {
                    new Zone
                    {
                        startPos = new Vector2(px, py),
                        endPos = new Vector2(sx, sy)
                    }
                };

                field.SetValue(currentSectionObject, zoneList);
            }

            else if (currentSectionName == "Perim")
            {
                var parts = trimmed.Split('=');
                if (parts.Length != 2) continue;

                var field = currentSectionType.GetField(parts[0]);
                if (field == null || field.FieldType != typeof(int)) continue;

                if (int.TryParse(parts[1], out int intVal))
                {
                    int converted = (int)((float)intVal / 16f);

                    // Apply subtraction from 107 for Bottom and Right
                    if (field.Name == "Bottom" || field.Name == "Right")
                    {
                        converted = 107 - converted;
                    }

                    field.SetValue(currentSectionObject, converted);
                }
            }

            // JOBS & INFO
            else if (currentSectionName == "Jobs" || currentSectionName == "Info")
            {
                var parts = trimmed.Split('=');
                if (parts.Length != 2) continue;

                var field = currentSectionType.GetField(parts[0]);
                if (field == null) continue;

                object value = null;

                if (field.FieldType == typeof(bool))
                {
                    // Handle "1" and "0" as bool
                    if (parts[1] == "1")
                        value = true;
                    else if (parts[1] == "0")
                        value = false;
                    else if (bool.TryParse(parts[1], out bool boolVal))
                        value = boolVal;
                    else
                        continue; // skip invalid boolean input
                }
                else if (field.FieldType == typeof(int))
                {
                    if (int.TryParse(parts[1], out int intVal))
                        value = intVal;
                    else
                        continue; // skip invalid int input
                }
                else if (field.FieldType == typeof(string))
                {
                    value = parts[1];
                }

                if (value != null)
                    field.SetValue(currentSectionObject, value);
            }

            // OBJECTS
            else if (currentSectionName == "Objects")
            {
                var parts = trimmed.Split('=');
                if (parts.Length != 2) continue;

                var values = parts[1].Split('x');
                if (values.Length != 4) continue;


                float px = float.Parse(values[0], CultureInfo.InvariantCulture);
                float py = float.Parse(values[1], CultureInfo.InvariantCulture);

                int id = int.Parse(values[2]);
                int layer = int.Parse(values[3]);

                layer = layer == 4 ? 0 : layer;

                properties.Objects ??= new List<Objects>();
                properties.Objects.Add(new Objects
                {
                    Id = id,
                    Position = new Vector2(px, py),
                    Layer = layer
                });
            }

            // TILES
            else if (tileLayers.TryGetValue(currentSectionName, out int tileLayer))
            {
                tileLayer += 1;
                tileLayer = tileLayer == 4 ? 0 : tileLayer;

                var parts = trimmed.Split('=');
                if (parts.Length != 2) continue;

                int y = -int.Parse(parts[0]);
                var ids = parts[1].Split('_');

                for (int x = 0; x < ids.Length; x++)
                {
                    if (string.IsNullOrWhiteSpace(ids[x]))
                    {
                        continue;
                    }

                    if (!int.TryParse(ids[x], out var id))
                    {
                        continue;
                    }

                    if (id == 0)
                    {
                        continue;
                    }

                    properties.Tiles ??= new List<Tiles>();
                    properties.Tiles.Add(new Tiles
                    {
                        Id = id,
                        Position = new Vector2Int(x, y),
                        Layer = tileLayer
                    });
                }
            }
        }
    }
}


// Properties

[System.Serializable]
public class Properties
{
    public Jobs Jobs;
    public Info Info;
    public Zones Zones;
    public Perim Perim;

    public List<Objects> Objects;
    public List<Tiles> Tiles;
    
    public Properties Clone()
    {
        return new Properties
        {
            Jobs = new Jobs { /* copy fields */ },
            Info = new Info { /* copy fields */ },
            Zones = new Zones { /* copy fields */ },
            Perim = new Perim { /* copy fields */ },
            Objects = new List<Objects>(Objects.Select(o => o.Clone())),
            Tiles = new List<Tiles>(Tiles.Select(t => t.Clone()))
        };
    }
}

[System.Serializable]
public class Jobs
{
    public bool Laundry;
    public bool Gardening;
    public bool Janitor;
    public bool Woodshop;
    public bool Metalshop;
    public bool Kitchen;
    public bool Deliveries;
    public bool Tailor;
    public bool Mailman;
    public bool Library;
    public string StartingJob;
    
    public Jobs Clone()
    {
        return new Jobs
        {
            Laundry = this.Laundry,
            Gardening = this.Gardening,
            Janitor = this.Janitor,
            Woodshop = this.Woodshop,
            Metalshop = this.Metalshop,
            Kitchen = this.Kitchen,
            Deliveries = this.Deliveries,
            Tailor = this.Tailor,
            Mailman = this.Mailman,
            Library = this.Library,
            StartingJob = this.StartingJob
        };
    }
}

[System.Serializable]
public class Info
{
    public int Custom;
    public int Rdy = 0;
    public string Version;
    public string MapName;
    public string Intro;
    public string Warden;
    public int Guards;
    public int Inmates;
    public string Tileset;
    public string Floor;
    public string Music;
    public string MapType;
    public string RoutineSet;
    public int NPClvl;
    
    public Info Clone()
    {
        return new Info
        {
            Custom = this.Custom,
            Rdy = this.Rdy,
            Version = this.Version,
            MapName = this.MapName,
            Intro = this.Intro,
            Warden = this.Warden,
            Guards = this.Guards,
            Inmates = this.Inmates,
            Tileset = this.Tileset,
            Floor = this.Floor,
            Music = this.Music,
            MapType = this.MapType,
            RoutineSet = this.RoutineSet,
            NPClvl = this.NPClvl
        };
    }
}

[System.Serializable]
public class Zones
{
    public List<Zone> Rollcall;
    public List<Zone> Canteen;
    public List<Zone> Gym;
    public List<Zone> Showers;

    public List<Zone> YourCell;
    public List<Zone> SHU;
    public List<Zone> Cells1;
    public List<Zone> Cells2;
    public List<Zone> Cells3;
    public List<Zone> Cells4;
    public List<Zone> Cells5;
    public List<Zone> Cells6;
    public List<Zone> Cells7;
    public List<Zone> Cells8;
    public List<Zone> Cells9;
    public List<Zone> Cells10;

    public List<Zone> Laundry;
    public List<Zone> Gardening;
    public List<Zone> Janitor;
    public List<Zone> Woodshop;
    public List<Zone> Metalshop;
    public List<Zone> Kitchen;
    public List<Zone> Deliveries;
    public List<Zone> Tailor;

    public List<Zone> Safe1;
    public List<Zone> Safe2;
    public List<Zone> Safe3;
    
    public Zones Clone()
    {
        return new Zones
        {
            Rollcall = this.Rollcall?.Select(z => z.Clone()).ToList(),
            Canteen = this.Canteen?.Select(z => z.Clone()).ToList(),
            Gym = this.Gym?.Select(z => z.Clone()).ToList(),
            Showers = this.Showers?.Select(z => z.Clone()).ToList(),

            YourCell = this.YourCell?.Select(z => z.Clone()).ToList(),
            SHU = this.SHU?.Select(z => z.Clone()).ToList(),
            Cells1 = this.Cells1?.Select(z => z.Clone()).ToList(),
            Cells2 = this.Cells2?.Select(z => z.Clone()).ToList(),
            Cells3 = this.Cells3?.Select(z => z.Clone()).ToList(),
            Cells4 = this.Cells4?.Select(z => z.Clone()).ToList(),
            Cells5 = this.Cells5?.Select(z => z.Clone()).ToList(),
            Cells6 = this.Cells6?.Select(z => z.Clone()).ToList(),
            Cells7 = this.Cells7?.Select(z => z.Clone()).ToList(),
            Cells8 = this.Cells8?.Select(z => z.Clone()).ToList(),
            Cells9 = this.Cells9?.Select(z => z.Clone()).ToList(),
            Cells10 = this.Cells10?.Select(z => z.Clone()).ToList(),

            Laundry = this.Laundry?.Select(z => z.Clone()).ToList(),
            Gardening = this.Gardening?.Select(z => z.Clone()).ToList(),
            Janitor = this.Janitor?.Select(z => z.Clone()).ToList(),
            Woodshop = this.Woodshop?.Select(z => z.Clone()).ToList(),
            Metalshop = this.Metalshop?.Select(z => z.Clone()).ToList(),
            Kitchen = this.Kitchen?.Select(z => z.Clone()).ToList(),
            Deliveries = this.Deliveries?.Select(z => z.Clone()).ToList(),
            Tailor = this.Tailor?.Select(z => z.Clone()).ToList(),

            Safe1 = this.Safe1?.Select(z => z.Clone()).ToList(),
            Safe2 = this.Safe2?.Select(z => z.Clone()).ToList(),
            Safe3 = this.Safe3?.Select(z => z.Clone()).ToList()
        };
    }
}

[System.Serializable]
public class Zone
{
    public Vector2 startPos;
    public Vector2 endPos;
    
    public Zone Clone()
    {
        return new Zone
        {
            startPos = this.startPos,
            endPos = this.endPos
        };
    }
}

[System.Serializable]
public class Perim
{
    public int Top;
    public int Bottom;
    public int Left;
    public int Right;
    
    public Perim Clone()
    {
        return new Perim
        {
            Top = this.Top,
            Bottom = this.Bottom,
            Left = this.Left,
            Right = this.Right
        };
    }
}

[System.Serializable]
public class MapData
{
    public List<Tiles> tiles;
    public List<Objects> objects;
    
    public MapData Clone()
    {
        return new MapData
        {
            tiles = this.tiles?.Select(t => t.Clone()).ToList(),
            objects = this.objects?.Select(o => o.Clone()).ToList()
        };
    }
}

[System.Serializable]
public class Tiles
{
    [FormerlySerializedAs("id")] public int Id;
    [FormerlySerializedAs("pos")] public Vector2Int Position;
    [FormerlySerializedAs("layer")] public int Layer;
    
    public Tiles Clone()
    {
        return new Tiles
        {
            Id = this.Id,
            Position = this.Position,
            Layer = this.Layer
        };
    }
}

[System.Serializable]
public class Objects
{
    [FormerlySerializedAs("id")] public int Id;
    [FormerlySerializedAs("pos")] public Vector2 Position;
    [FormerlySerializedAs("layer")] public int Layer;
    
    public Objects Clone()
    {
        return new Objects
        {
            Id = this.Id,
            Position = this.Position,
            Layer = this.Layer
        };
    }
}
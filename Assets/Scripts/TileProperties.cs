using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    public static TileProperties instance;

    // Credit: https://drive.google.com/drive/folders/11DS6T9qxpY3FfKGmiJ3Xdt4qo-9cDj7P
    
    List<string> perks = new List<string>
    {
        "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall",
        "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor (Indoors)", "Floor (Outdoors)", "Fence (cuttable)",
        "Fence (cuttable)",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor (Indoors)", "Obstacle", "Vent Wall (north)",
        "Vent Wall (west)",
        "Vent Wall (east)", "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging",
        "Roof edging", "Roof edging", "Water (Obstacle)", "Water (Obstacle)", "Vent Wall Corner", "Vent Wall Corner",
        "Roof layer (low) (Indoors)", "Roof layer (med) (Indoors)", "Roof layer (high) (Indoors)", "Roof pipe (Indoors)", "Roof pipe (Indoors)", "Water (Obstacle)",
        "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Indoors)", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Floor (Indoors)",
        "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)",
        "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)",
        "Water (Obstacle)",
        "-", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall",
        "Outer Wall",
        "Outer Wall", "Floor (Indoors)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)",
        "Electric Fence",
        "Electric Fence", "Reinforced Concrete", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Obstacle",
        "Floor (Indoors)", "Floor (Indoors)"
    };

    List<string> stalagflucht = new List<string>
    {
        "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall",
        "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor (Indoors)", "Floor (Outdoors)", "Fence (cuttable)",
        "Fence (cuttable)",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor (Indoors)", "Obstacle", "Vent Wall (north)",
        "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging",
        "Roof edging", "Obstacle", "Obstacle",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low) (Indoors)", "Roof layer (med) (Indoors)", "Roof layer (high) (Indoors)",
        "Roof pipe (Indoors)", "Roof pipe (Indoors)", "Obstacle", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Indoors)",
        "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Floor (Indoors)",
        "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Floor (Indoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Obstacle", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Roof (Indoors)", "Outer Wall", "Outer Wall", "Outer Wall",
        "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Floor (Indoors)",
        "Wall Mask (roof layer)", "Wall Mask (roof layer)",
        "Wall Mask (roof layer)", "Electric Fence", "Electric Fence", "Reinforced Concrete", "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)",
        "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)"
    };

    List<string> shanktonstatepen = new List<string>
    {
        "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Wall", "Floor (Indoors)", "Floor (Outdoors)", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor (Indoors)", "Obstacle",
        "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging",
        "Roof edging", "Obstacle", "Floor (Indoors)",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low) (Indoors)", "Roof layer (med) (Indoors)", "Roof layer (high) (Indoors)",
        "Roof pipe (Indoors)", "Roof pipe (Indoors)", "Obstacle", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Indoors)",
        "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle",
        "Obstacle", "Floor (Indoors)",
        "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Water (obstacle)", "Water (obstacle)", "Water (obstacle)",
        "Water (obstacle)", "Water (obstacle)", "Water (obstacle)",
        "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall",
        "Outer Wall", "Outer Wall",
        "Floor (Indoors)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence",
        "Electric Fence", "Reinforced Concrete", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)",
        "Floor (Indoors)", "Water (obstacle)", "Water (obstacle)", "Water (obstacle)", "Water (obstacle"
    };
    
    List<string> jungle = new List<string>
    {
        "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall (Chippable)", "Wall (Chippable)", "Floor (Indoors)", "Floor (Outdoors)", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor (Indoors)", "Obstacle", "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Wall", "Obstacle",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low) (Indoors)", "Roof layer (med) (Indoors)", "Roof layer (high) (Indoors)", "Roof pipe (Indoors)", "Roof pipe (Indoors)", "Obstacle", "Obstacle", "Floor (Outdoors)",
        "Floor (Indoors)", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Wall",
        "Wall", "Wall", "Wall", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Obstacle", "Obstacle", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Obstacle", "Roof",
        "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall",
        "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence", "Electric Fence", "Reinforced Concrete", "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)",
        "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)", "Roof (Indoors)"
    };
    
    List<string> sanpancho = new List<string>
    {
        "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor (Indoors)", "Floor (Outdoors)", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor (Indoors)", "Obstacle", "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Obstacle", "Obstacle",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low) (Indoors)", "Roof layer (med) (Indoors)", "Roof layer (high) (Indoors)", "Roof pipe (Indoors)", "Roof pipe (Indoors)", "Obstacle", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Indoors)",
        "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Obstacle", "Floor (Indoors)", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall",
        "Obstacle", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence",
        "Electric Fence", "Reinforced Concrete", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)"
    };
    
    List<string> irongate = new List<string>
    {
        "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor (Indoors)", "Floor (Outdoors)", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor (Indoors)", "Obstacle", "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Water (Obstacle)", "Water (Obstacle)",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low) (Indoors)", "Roof layer (med) (Indoors)", "Roof layer (high) (Indoors)", "Roof pipe (Indoors)", "Roof pipe (Indoors)", "Water (Obstacle)", "Floor (Outdoors)", "Floor (Outdoors)",
        "Floor (Indoors)", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle",
        "Floor (Indoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Floor (Outdoors)", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)",
        "Water (Obstacle)", "Water (Obstacle)", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall",
        "Outer Wall", "Outer Wall", "Floor (Indoors)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence", "Electric Fence", "Reinforced Concrete", "Floor (Indoors)", "Floor (Indoors)", "Floor (Indoors)",
        "Floor (Indoors)", "Floor (Indoors)", "Obstacle", "Obstacle", "Floor (Indoors)"
    };

    public List<string> currentProperties = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        currentProperties = perks;
        instance = this;
    }

    public void GetProperties()
    {
        if (MapProperties.instance.properties.Info.Tileset == "perks")
        {
            for (int i = 0; i < 100; i++)
            {
                transform.GetChild(0).transform.GetChild(i).gameObject.name = perks[ConvertXtoY(i + 1, 4, 25) - 1];
            }

            currentProperties = perks;
        }

        if (MapProperties.instance.properties.Info.Tileset == "stalagflucht")
        {
            for (int i = 0; i < 100; i++)
            {
                transform.GetChild(0).transform.GetChild(i).gameObject.name =
                    stalagflucht[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            
            currentProperties = stalagflucht;
        }

        if (MapProperties.instance.properties.Info.Tileset == "shanktonstatepen")
        {
            for (int i = 0; i < 100; i++)
            {
                transform.GetChild(0).transform.GetChild(i).gameObject.name =
                    shanktonstatepen[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            
            currentProperties = shanktonstatepen;
        }
        
        if (MapProperties.instance.properties.Info.Tileset == "jungle")
        {
            for (int i = 0; i < 100; i++)
            {
                transform.GetChild(0).transform.GetChild(i).gameObject.name =
                    jungle[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            currentProperties = jungle;
        }
        
        if (MapProperties.instance.properties.Info.Tileset == "sanpancho")
        {
            for (int i = 0; i < 100; i++)
            {
                transform.GetChild(0).transform.GetChild(i).gameObject.name =
                    sanpancho[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            currentProperties = sanpancho;
        }
        
        if (MapProperties.instance.properties.Info.Tileset == "irongate")
        {
            for (int i = 0; i < 100; i++)
            {
                transform.GetChild(0).transform.GetChild(i).gameObject.name =
                    irongate[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            currentProperties = irongate;
        }
    }

    public int ConvertXtoY(int id, int width, int height)
    {
        // Convert to 0-based index
        int zeroBasedId = id - 1;

        // Get x and y position in row-major (X-incremented)
        int x = zeroBasedId % width;
        int y = zeroBasedId / width;

        // Convert to column-major (Y-incremented)
        int newId = y + x * height + 1;

        return newId;
    }
}
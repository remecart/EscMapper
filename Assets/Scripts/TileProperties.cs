using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    public static TileProperties instance;

    List<string> perks = new List<string>
    {
        "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall",
        "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor", "Floor", "Fence (cuttable)",
        "Fence (cuttable)",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor", "Obstacle", "Vent Wall (north)",
        "Vent Wall (west)",
        "Vent Wall (east)", "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging",
        "Roof edging", "Roof edging", "Water (Obstacle)", "Water (Obstacle)", "Vent Wall Corner", "Vent Wall Corner",
        "Roof layer (low)", "Roof layer (med)", "Roof layer (high)", "Roof pipe", "Roof pipe", "Water (Obstacle)",
        "Floor", "Floor", "Floor", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Floor",
        "Floor", "Floor", "Floor", "Floor",
        "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)",
        "Water (Obstacle)",
        "-", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall",
        "Outer Wall",
        "Outer Wall", "Floor", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)",
        "Electric Fence",
        "Electric Fence", "Reinforced Concrete", "Floor", "Floor", "Floor", "Floor", "Floor", "Floor", "Obstacle",
        "Floor", "Floor"
    };

    List<string> stalagflucht = new List<string>
    {
        "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall",
        "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor", "Floor", "Fence (cuttable)",
        "Fence (cuttable)",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor", "Obstacle", "Vent Wall (north)",
        "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging",
        "Roof edging", "Obstacle", "Obstacle",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low)", "Roof layer (med)", "Roof layer (high)",
        "Roof pipe", "Roof pipe", "Obstacle", "Floor", "Floor", "Floor",
        "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Floor",
        "Floor", "Floor", "Obstacle", "Floor", "Floor", "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Roof", "Outer Wall", "Outer Wall", "Outer Wall",
        "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Floor",
        "Wall Mask (roof layer)", "Wall Mask (roof layer)",
        "Wall Mask (roof layer)", "Electric Fence", "Electric Fence", "Reinforced Concrete", "Roof", "Roof", "Roof",
        "Roof", "Roof", "Roof", "Roof", "Roof"
    };

    List<string> shanktonstatepen = new List<string>
    {
        "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Wall", "Floor", "Floor", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor", "Obstacle",
        "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging",
        "Roof edging", "Obstacle", "Floor",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low)", "Roof layer (med)", "Roof layer (high)",
        "Roof pipe", "Roof pipe", "Obstacle", "Floor", "Floor", "Floor",
        "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle",
        "Obstacle", "Floor",
        "Floor", "Floor", "Floor", "Floor", "Water (obstacle)", "Water (obstacle)", "Water (obstacle)",
        "Water (obstacle)", "Water (obstacle)", "Water (obstacle)",
        "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall",
        "Outer Wall", "Outer Wall",
        "Floor", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence",
        "Electric Fence", "Reinforced Concrete", "Floor", "Floor", "Floor",
        "Floor", "Water (obstacle)", "Water (obstacle)", "Water (obstacle)", "Water (obstacle"
    };
    
    List<string> jungle = new List<string>
    {
        "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall (Chippable)", "Wall (Chippable)", "Floor", "Floor", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor", "Obstacle", "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Wall", "Obstacle",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low)", "Roof layer (med)", "Roof layer (high)", "Roof pipe", "Roof pipe", "Obstacle", "Obstacle", "Floor",
        "Floor", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Wall",
        "Wall", "Wall", "Wall", "Floor", "Floor", "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Obstacle", "Roof",
        "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall",
        "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence", "Electric Fence", "Reinforced Concrete", "Roof", "Roof", "Roof",
        "Roof", "Roof", "Roof", "Roof", "Roof"
    };
    
    List<string> sanpancho = new List<string>
    {
        "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor", "Floor", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor", "Obstacle", "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Obstacle", "Obstacle",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low)", "Roof layer (med)", "Roof layer (high)", "Roof pipe", "Roof pipe", "Obstacle", "Floor", "Floor", "Floor",
        "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Floor", "Floor", "Floor", "Floor", "Obstacle", "Obstacle",
        "Obstacle", "Obstacle", "Obstacle", "Floor", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall",
        "Obstacle", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence",
        "Electric Fence", "Reinforced Concrete", "Floor", "Floor", "Floor", "Floor", "Floor", "Floor", "Floor", "Floor"
    };
    
    List<string> irongate = new List<string>
    {
        "Floor", "Floor", "Floor", "Obstacle", "Obstacle", "Wall (Chippable)", "Wall (Chippable)", "Wall", "Wall", "Wall",
        "Wall", "Wall", "Wall", "Wall", "Wall", "Wall", "Obstacle", "Floor", "Floor", "Fence (cuttable)",
        "Fence (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Vent Floor", "Obstacle", "Vent Wall (north)", "Vent Wall (west)", "Vent Wall (east)",
        "Vent Wall (south)", "Roof edging", "Roof edging", "Obstacle", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Water (Obstacle)", "Water (Obstacle)",
        "Vent Wall Corner", "Vent Wall Corner", "Roof layer (low)", "Roof layer (med)", "Roof layer (high)", "Roof pipe", "Roof pipe", "Water (Obstacle)", "Floor", "Floor",
        "Floor", "Roof edging", "Roof edging", "Roof edging", "Roof edging", "Bars (cuttable)", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle", "Obstacle",
        "Floor", "Floor", "Floor", "Floor", "Floor", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)", "Water (Obstacle)",
        "Water (Obstacle)", "Water (Obstacle)", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Outer Wall", "Obstacle", "Outer Wall", "Outer Wall",
        "Outer Wall", "Outer Wall", "Floor", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Wall Mask (roof layer)", "Electric Fence", "Electric Fence", "Reinforced Concrete", "Floor", "Floor", "Floor",
        "Floor", "Floor", "Obstacle", "Obstacle", "Floor"
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
            for (int i = 0; i < transform.GetChild(1).transform.childCount; i++)
            {
                transform.GetChild(1).transform.GetChild(i).gameObject.name = perks[ConvertXtoY(i + 1, 4, 25) - 1];
            }

            currentProperties = perks;
        }

        if (MapProperties.instance.properties.Info.Tileset == "stalagflucht")
        {
            for (int i = 0; i < transform.GetChild(1).transform.childCount; i++)
            {
                transform.GetChild(1).transform.GetChild(i).gameObject.name =
                    stalagflucht[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            
            currentProperties = stalagflucht;
        }

        if (MapProperties.instance.properties.Info.Tileset == "shanktonstatepen")
        {
            for (int i = 0; i < transform.GetChild(1).transform.childCount; i++)
            {
                transform.GetChild(1).transform.GetChild(i).gameObject.name =
                    shanktonstatepen[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            
            currentProperties = shanktonstatepen;
        }
        
        if (MapProperties.instance.properties.Info.Tileset == "jungle")
        {
            for (int i = 0; i < transform.GetChild(1).transform.childCount; i++)
            {
                transform.GetChild(1).transform.GetChild(i).gameObject.name =
                    jungle[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            currentProperties = jungle;
        }
        
        if (MapProperties.instance.properties.Info.Tileset == "sanpancho")
        {
            for (int i = 0; i < transform.GetChild(1).transform.childCount; i++)
            {
                transform.GetChild(1).transform.GetChild(i).gameObject.name =
                    sanpancho[ConvertXtoY(i + 1, 4, 25) - 1];
            }
            currentProperties = sanpancho;
        }
        
        if (MapProperties.instance.properties.Info.Tileset == "irongate")
        {
            for (int i = 0; i < transform.GetChild(1).transform.childCount; i++)
            {
                transform.GetChild(1).transform.GetChild(i).gameObject.name =
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
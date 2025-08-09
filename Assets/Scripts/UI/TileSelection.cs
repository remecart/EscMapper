using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TileSelection : MonoBehaviour
{
    public static TileSelection instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    
    // Update is called once per frame
    // ReSharper disable Unity.PerformanceAnalysis
    public void ReloadPageTextures()
    {
        for (int i = 1; i <= transform.childCount; i++)
        {
            int convertedId = ConvertXtoY(i, 4, 25) - 1;
            if (convertedId >= 0 && convertedId < TextureManagement.instance.loadedTiles.Count)
            {
                transform.GetChild(i - 1).gameObject.GetComponent<RawImage>().texture =
                    TextureManagement.instance.loadedTiles[convertedId];
            }
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

#if UNITY_EDITOR
[CustomEditor(typeof(TileSelection)), System.Serializable]
public class TileSelectionInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TileSelection script = (TileSelection)target;

        if (GUILayout.Button("Reload Page"))
        {
            script.ReloadPageTextures();
        }
    }
}
#endif

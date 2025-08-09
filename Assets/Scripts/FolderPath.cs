using System;
using System.IO;
using SFB;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FolderPath : MonoBehaviour
{
    public static FolderPath instance;

    public GameObject UI;

    public Config Config;
    public string path;
    
    public TMP_InputField sourcePath;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        instance = this;

        // Use Path.Combine for cross-platform compatibility
        string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EscMapper", "config.json");

        if (File.Exists(configPath))
        {
            string raw = File.ReadAllText(configPath);
            Config = JsonUtility.FromJson<Config>(raw);

            // Check if the directory exists
            if (Config == null)
            {
                UI.gameObject.SetActive(true);
                return;
            }
            
            string sourceFolderPath = Config.sourceFolderPath;
            
            if (!Directory.Exists(sourceFolderPath))
            {
                UI.gameObject.SetActive(true);
            }
        }
        else
        {
            UI.gameObject.SetActive(true);
        }
    }

    public void SourceFolderPath()
    {
        var path = StandaloneFileBrowser.OpenFolderPanel("Select Game folder", "", false)[0];
        
        if (Directory.Exists(path))
        {
            // Extract folder name from path
            string folderName = Path.GetFileName(Path.GetFullPath(path));

            if (folderName == "The Escapists")
            {
                UI.transform.GetChild(0).gameObject.SetActive(false);
                sourcePath.text = path;
            }
            else
            {
                UI.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void Abort()
    {
        var path = sourcePath.text;
        var folderName = Path.GetFileName(Path.GetFullPath(path));
        
        if (Directory.Exists(path) && folderName == "The Escapists")
        {
            UI.SetActive(false);
            Config = new Config();
            Config.sourceFolderPath = path;
        }
        else
        {
            UI.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string savepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EscMapper", "config.json");
        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EscMapper"));
        File.WriteAllText(savepath, JsonUtility.ToJson(Config));
    }
}

public class Config
{
    public string sourceFolderPath;
}

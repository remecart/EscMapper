using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CheckLatestReleases : MonoBehaviour
{
    private const string ApiUrl =
        "https://api.github.com/repos/remecart/EscMapper/releases/latest";

    public List<TextMeshProUGUI> texts;

    public GameObject UI;
    
    [Serializable]
    private class GitHubRelease
    {
        public string tag_name;
        public string html_url;
    }

    private void Start()
    {
        StartCoroutine(CheckForUpdates());
    }

    private IEnumerator CheckForUpdates()
    {
        using UnityWebRequest request = UnityWebRequest.Get(ApiUrl);

        request.SetRequestHeader("User-Agent", "Unity");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Update check failed: {request.error}");
            yield break;
        }

        GitHubRelease release =
            JsonUtility.FromJson<GitHubRelease>(request.downloadHandler.text);

        if (release == null)
        {
            Debug.LogError("Failed to parse GitHub release data.");
            yield break;
        }

        if (string.IsNullOrEmpty(release.tag_name))
        {
            Debug.LogError("GitHub response did not contain a tag_name.");
            yield break;
        }

        string latestVersion = release.tag_name.TrimStart('v');

        if (!Version.TryParse(latestVersion, out Version latest))
        {
            Debug.LogError($"Could not parse latest version: {latestVersion}");
            yield break;
        }

        if (!Version.TryParse(Application.version, out Version current))
        {
            Debug.LogError($"Could not parse current version: {Application.version}");
            yield break;
        }

        if (latest > current)
        {
            UI.gameObject.SetActive(true);
            texts[0].text = $"There is a new update available:\nv{Application.version} => v{latest}";
            
            // Debug.Log($"Update available: {latest}");
            // Debug.Log($"Download: {release.html_url}");

            // Show update UI here
        }
    }
}
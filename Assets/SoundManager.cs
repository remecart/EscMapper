using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource source;
    public List<AudioClip> sounds;
    public List<string> names;
    public int maxActiveSounds = 15;
    public Slider slider;
    public float volume;

    private List<AudioSource> activeSources = new List<AudioSource>();
    private List<AudioSource> pool = new List<AudioSource>();

    public static SoundManager instance;

    private void Start()
    {
        instance = this;
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.1f);
        MyFunction();
    }

    void MyFunction()
    {
        volume = FolderPath.instance.Config.volume;
    }

    public void ChangeVolume()
    {
        volume = slider.value;
    }

    public void PlayButton() => PlaySound("Button");
    public void PlayToggle() => PlaySound("Toggle");

    public void PlaySound(string name)
    {
        int index = names.IndexOf(name);
        if (index < 0 || index >= sounds.Count) return;

        AudioClip clip = sounds[index];

        // Clean up finished sources
        activeSources.RemoveAll(s => !s.isPlaying);

        // Limit active sources
        if (activeSources.Count >= maxActiveSounds)
        {
            AudioSource oldest = activeSources[0];
            oldest.Stop();
            activeSources.RemoveAt(0);
        }

        // Get source from pool or create new
        // AudioSource source = pool.Count > 0 ? pool[0] : gameObject.AddComponent<AudioSource>();
        if (pool.Count > 0) pool.RemoveAt(0);

        source.clip = clip;
        source.Play();
        activeSources.Add(source);

        // Scale all active sources so total volume <= 1
        float scale = 1f / activeSources.Count;
        foreach (var s in activeSources)
            s.volume = Mathf.Clamp(scale, 0.05f, 1f) * volume;

        StartCoroutine(ReturnToPoolAfterPlay(source));
    }

    private IEnumerator ReturnToPoolAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        activeSources.Remove(source);
        pool.Add(source);

        // Rescale remaining sources
        if (activeSources.Count > 0)
        {
            float scale = 1f / activeSources.Count;
            foreach (var s in activeSources)
                s.volume = Mathf.Clamp(scale, 0.05f, 1f) * volume;
        }
    }
}
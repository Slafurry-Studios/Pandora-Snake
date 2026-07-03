using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 16;

    private AudioSource[] pool;
    private int oldestIndex = 0;

    // Menyimpan SoundEffect apa yang sedang dimainkan oleh setiap AudioSource
    private readonly Dictionary<AudioSource, string> playingSounds = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitPool();
    }

    private void InitPool()
    {
        pool = new AudioSource[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = new GameObject($"SFX_Source_{i}");
            go.transform.SetParent(transform);

            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = sfxMixerGroup;

            pool[i] = source;
        }
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var source in pool)
        {
            if (!source.isPlaying)
            {
                playingSounds.Remove(source);
                return source;
            }
        }

        AudioSource stolen = pool[oldestIndex];

        stolen.Stop();
        playingSounds.Remove(stolen);

        oldestIndex = (oldestIndex + 1) % poolSize;

        return stolen;
    }

    private int CountPlaying(string soundName)
    {
        int count = 0;

        foreach (var source in pool)
        {
            if (!source.isPlaying)
            {
                playingSounds.Remove(source);
                continue;
            }

            if (playingSounds.TryGetValue(source, out string currentName))
            {
                if (currentName == soundName)
                    count++;
            }
        }

        return count;
    }

    // =========================
    // 3D
    // =========================

    public void PlaySound3D(AudioClip clip, Vector3 pos, float volume = 1f)
    {
        if (clip == null)
            return;

        AudioSource source = GetAvailableSource();

        source.transform.position = pos;
        source.spatialBlend = 1f;
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        SoundEffect sfx = sfxLibrary.GetSoundEffect(soundName);

        if (sfx == null || sfx.clips == null || sfx.clips.Length == 0)
        {
            Debug.LogWarning($"[SoundManager] Sound '{soundName}' tidak ditemukan.");
            return;
        }

        if (CountPlaying(soundName) >= sfx.maxSimultaneous)
            return;

        AudioClip clip = sfx.clips[Random.Range(0, sfx.clips.Length)];

        AudioSource source = GetAvailableSource();

        source.transform.position = pos;
        source.spatialBlend = 1f;
        source.clip = clip;
        source.volume = sfx.volume;

        playingSounds[source] = soundName;

        source.Play();
    }

    // =========================
    // 2D
    // =========================

    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return;

        AudioSource source = GetAvailableSource();

        source.transform.position = Vector3.zero;
        source.spatialBlend = 0f;
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }

    public void PlaySound2D(string soundName)
    {
        SoundEffect sfx = sfxLibrary.GetSoundEffect(soundName);

        if (sfx == null || sfx.clips == null || sfx.clips.Length == 0)
        {
            Debug.LogWarning($"[SoundManager] Sound '{soundName}' tidak ditemukan.");
            return;
        }

        if (CountPlaying(soundName) >= sfx.maxSimultaneous)
            return;

        AudioClip clip = sfx.clips[Random.Range(0, sfx.clips.Length)];

        AudioSource source = GetAvailableSource();

        source.transform.position = Vector3.zero;
        source.spatialBlend = 0f;
        source.clip = clip;
        source.volume = sfx.volume;

        playingSounds[source] = soundName;

        source.Play();
    }
}
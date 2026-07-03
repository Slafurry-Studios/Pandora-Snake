using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Pool Settings")]
    [SerializeField]
    private int poolSize = 16;

    private AudioSource[] pool;
    private int oldestIndex = 0;

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
        // Cari yang idle dulu
        foreach (var source in pool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // Kalau semua sibuk, steal yang paling lama
        AudioSource stolen = pool[oldestIndex];
        stolen.Stop();
        oldestIndex = (oldestIndex + 1) % poolSize;
        return stolen;
    }

    // ── 3D ──────────────────────────────────────────────

    public void PlaySound3D(AudioClip clip, Vector3 pos, float volume = 1f)
    {
        if (clip == null) return;

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
        if (sfx.clips != null && sfx.clips.Length > 0)
        {
            AudioClip clip = sfx.clips[Random.Range(0, sfx.clips.Length)];
            PlaySound3D(clip, pos, sfx.volume);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Sound '{soundName}' tidak ditemukan di SoundLibrary.");
        }
    }

    // ── 2D ──────────────────────────────────────────────

    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

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
        if (sfx.clips != null && sfx.clips.Length > 0)
        {
            AudioClip clip = sfx.clips[Random.Range(0, sfx.clips.Length)];
            PlaySound2D(clip, sfx.volume);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Sound '{soundName}' tidak ditemukan di SoundLibrary.");
        }
    }
}
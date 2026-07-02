using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    [System.Serializable]
    public struct SceneTrack
    {
        public string sceneName;
        public string trackName;
    }

    [Header("Scene Music")]
    [SerializeField]
    private SceneTrack[] sceneTracks;

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string trackToPlay = null;
        if (sceneTracks != null)
        {
            foreach (var st in sceneTracks)
            {
                if (!string.IsNullOrEmpty(st.sceneName) && st.sceneName == scene.name)
                {
                    trackToPlay = st.trackName;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(trackToPlay))
        {
            trackToPlay = scene.name;
        }

        if (musicLibrary != null && musicLibrary.GetClipFromName(trackToPlay) != null)
        {
            PlayMusic(trackToPlay);
        }
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        if (musicLibrary == null)
        {
            return;
        }

        MusicTrack track = musicLibrary.GetTrack(trackName);
        if (track.clip == null)
        {
            return;
        }

        if (musicSource == null)
        {
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(AnimateMusicCrossfade(track.clip, track.volume, fadeDuration));
    }

    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float targetVolume, float fadeDuration = 0.5f)
    {
        float startVolume = musicSource.volume;
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.unscaledDeltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(startVolume, 0, percent);
            yield return null;
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.unscaledDeltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, targetVolume, percent);
            yield return null;
        }

        musicSource.volume = targetVolume;
        currentFadeCoroutine = null;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TMPFadeArray : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TMP_Text textComponent;

    [Header("Content")]
    [TextArea(1, 3)]
    [SerializeField] private string[] texts;

    [Header("Typewriter")]
    [SerializeField] private bool useTypewriter = true;
    [SerializeField] private float charInterval = 0.03f;

    [Header("Fade")]
    [SerializeField] private float fadeInDuration = 0.4f;
    [SerializeField] private float fadeOutDuration = 0.6f;

    [Header("Timing")]
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private float delayBeforeStart = 0f;

    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typeLoopSfx;

    [Header("Options")]
    [SerializeField] private bool loop = true;
    [SerializeField] private bool playOnStart = true;

    [Header("Events")]
    public UnityEvent onTextEnd;
    public UnityEvent onSequenceEnd;

    private Coroutine _routine;

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(FadeRoutine());
    }

    public void Stop()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        StopTypeSfx();
    }

    public void ChangeScene(string newScene)
    {
        Stop();
        UnityEngine.SceneManagement.SceneManager.LoadScene(newScene);
    }

    private IEnumerator FadeRoutine()
    {
        if (textComponent == null || texts == null || texts.Length == 0)
        {
            Debug.LogWarning("TMPFadeArray: textComponent or texts array is not set.");
            yield break;
        }

        if (delayBeforeStart > 0f)
            yield return new WaitForSeconds(delayBeforeStart);

        int index = 0;

        do
        {
            textComponent.text = texts[index];
            textComponent.ForceMeshUpdate();

            int totalChars = textComponent.textInfo.characterCount;
            textComponent.maxVisibleCharacters = useTypewriter ? 0 : totalChars;
            SetAlpha(0f);

            yield return FadeTo(0f, 1f, fadeInDuration);

            if (useTypewriter)
                yield return TypeText(totalChars);

            yield return new WaitForSeconds(holdDuration);

            yield return FadeTo(1f, 0f, fadeOutDuration);

            onTextEnd?.Invoke();

            index++;
            if (index >= texts.Length)
                index = 0;

        } while (loop || index != 0);

        onSequenceEnd?.Invoke();
        _routine = null;
    }

    private IEnumerator TypeText(int totalChars)
    {
        PlayTypeSfx();

        int visible = 0;
        while (visible < totalChars)
        {
            visible++;
            textComponent.maxVisibleCharacters = visible;
            yield return new WaitForSeconds(charInterval);
        }

        StopTypeSfx();
    }

    private void PlayTypeSfx()
    {
        if (audioSource == null || typeLoopSfx == null)
            return;

        audioSource.clip = typeLoopSfx;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopTypeSfx()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
    }

    private IEnumerator FadeTo(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        Color c = textComponent.color;
        c.a = alpha;
        textComponent.color = c;
    }
}
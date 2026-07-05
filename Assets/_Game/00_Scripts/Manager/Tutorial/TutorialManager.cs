using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct TutorialPage
{
    public GameObject page;
    public UnityEvent OnShown;
    public UnityEvent OnHide;
}

[System.Serializable]
public struct TutorialSequence
{
    public string tutorialId;
    public GameObject parent;
    public TutorialPage[] tutorialPages;
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialSequence[] tutorials;

    private int currentSequenceIndex = -1;
    private int currentPageIndex = -1;

    private void Awake()
    {
        foreach (var sequence in tutorials)
        {
            SetPagesActive(sequence.tutorialPages, false);
            if (sequence.parent != null)
                sequence.parent.SetActive(false);
        }
    }

    public void StartTutorial(string tutorialId)
    {
        if (currentSequenceIndex != -1)
        {
            Debug.LogWarning($"[TutorialManager] Tutorial '{tutorials[currentSequenceIndex].tutorialId}' masih aktif, tutup dulu sebelum memulai tutorial baru.");
            return;
        }

        int index = FindSequenceIndex(tutorialId);
        if (index == -1)
        {
            Debug.LogWarning($"[TutorialManager] Tutorial dengan ID '{tutorialId}' tidak ditemukan.");
            return;
        }

        if (HasSeenTutorial(tutorialId))
        {
            Debug.Log($"[TutorialManager] Tutorial '{tutorialId}' sudah pernah ditonton, dilewati.");
            return;
        }

        var sequence = tutorials[index];
        if (sequence.tutorialPages.Length == 0)
        {
            Debug.LogWarning($"[TutorialManager] Tutorial '{tutorialId}' tidak memiliki halaman.");
            return;
        }

        currentSequenceIndex = index;
        currentPageIndex = 0;

        Time.timeScale = 0f;

        if (sequence.parent != null)
            sequence.parent.SetActive(true);

        SetPageActive(sequence.tutorialPages, currentPageIndex, true);
    }

    public void NextPage()
    {
        if (currentSequenceIndex == -1)
        {
            Debug.LogWarning("[TutorialManager] Tidak ada tutorial yang sedang aktif.");
            return;
        }

        var currentSequence = tutorials[currentSequenceIndex];

        SetPageActive(currentSequence.tutorialPages, currentPageIndex, false);

        currentPageIndex++;

        if (currentPageIndex < currentSequence.tutorialPages.Length)
        {
            SetPageActive(currentSequence.tutorialPages, currentPageIndex, true);
            return;
        }

        if (currentSequence.parent != null)
            currentSequence.parent.SetActive(false);

        MarkTutorialSeen(currentSequence.tutorialId);

        currentSequenceIndex = -1;
        currentPageIndex = -1;

        Time.timeScale = 1f;
    }

    private int FindSequenceIndex(string tutorialId)
    {
        for (int i = 0; i < tutorials.Length; i++)
        {
            if (tutorials[i].tutorialId == tutorialId)
                return i;
        }
        return -1;
    }

    private void SetPageActive(TutorialPage[] pages, int index, bool active)
    {
        if (pages == null || index < 0 || index >= pages.Length) return;

        var page = pages[index];
        if (page.page != null)
            page.page.SetActive(active);

        if (active)
            page.OnShown?.Invoke();
        else
            page.OnHide?.Invoke();
    }

    private void SetPagesActive(TutorialPage[] pages, bool active)
    {
        if (pages == null) return;

        foreach (var page in pages)
        {
            if (page.page != null)
                page.page.SetActive(active);
        }
    }

    private bool HasSeenTutorial(string id) => PlayerPrefs.GetInt("tutorial_" + id, 0) == 1;

    private void MarkTutorialSeen(string id) => PlayerPrefs.SetInt("tutorial_" + id, 1);
}
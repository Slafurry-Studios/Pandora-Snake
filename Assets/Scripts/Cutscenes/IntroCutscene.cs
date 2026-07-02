using System.Collections;
using Game.Dialog;
using Game.UI.HUD;
using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private string firstDialogBucket;
    [SerializeField] private string secondDialogBucket;
    [SerializeField] private string thirdDialogBucket;
    [SerializeField] private string fourthDialogBucket;
    [SerializeField] private ObjectiveScriptableObject firstObjective;
    [SerializeField] private ObjectiveScriptableObject[] mainObjectives;

    private DialogHUD dialogHUD;
    private DialogManager dialogManager;
    public int CurrentSequence = 3;

    private IEnumerator Start()
    {
        Time.timeScale = 0f;
        yield return new WaitUntil(() => PlayerManager.Instance != null);

        yield return new WaitUntil(() => FindAnyObjectByType<DialogHUD>() != null);
        dialogHUD = FindAnyObjectByType<DialogHUD>();
        dialogHUD.OnDialogEnd += () => NextCutscene();

        yield return new WaitUntil(() => FindAnyObjectByType<DialogManager>() != null);
        dialogManager = FindAnyObjectByType<DialogManager>();

        NextCutscene();
    }

    public void NextCutscene()
    {
        // Debug.Log($"CurrentSequence: {CurrentSequence}");
        if (CurrentSequence == 0)
        {
            CurrentSequence++;
            dialogManager.StartDialog(firstDialogBucket);
        }
        else if (CurrentSequence == 1)
        {
            CurrentSequence++;
            StartCoroutine(ChatIntroSequence());
            Time.timeScale = 0f;
            PlayerManager.Instance.Pause();
        }
        else if (CurrentSequence == 2)
        {
            CurrentSequence++;
            dialogManager.StartDialog(secondDialogBucket);
        }
        else if (CurrentSequence == 3)
        {
            CurrentSequence++;
            Time.timeScale = 0f;
            PlayerManager.Instance.Pause();
            StartCoroutine(MissionIntroSequence());
        }
        else if (CurrentSequence == 4)
        {
            CurrentSequence++;
            dialogManager.StartDialog(thirdDialogBucket);
        }
        else if (CurrentSequence == 5)
        {
            CurrentSequence++;
        }
        else if (CurrentSequence == 6)
        {
            CurrentSequence++;
            dialogManager.StartDialog(fourthDialogBucket);

        }
        else if (CurrentSequence == 7)
        {
            CurrentSequence++;
        }
        else if (CurrentSequence == 8)
        {
            foreach (ObjectiveScriptableObject objective in mainObjectives)
            {
                ObjectiveManager.Instance.AddObjective(objective.Objective);
            }
            PlayerManager.Instance.StatHUD.SetActive(true);
            PlayerManager.Instance.PauseHUD.SetActive(true);
            PlayerManager.Instance.ThreatHUD.SetActive(true);
        }
    }

    private IEnumerator ChatIntroSequence()
    {
        PlayerManager.Instance.ChatHUD.SetActive(true);

        StreamChatRoller chatRoller = PlayerManager.Instance.ChatHUD.GetComponentInChildren<StreamChatRoller>();

        yield return new WaitForSecondsRealtime(1f);
        chatRoller.AddChatMessage("dvfb", "FIRST!");

        yield return new WaitForSecondsRealtime(1f);
        chatRoller.AddChatMessage("tida", "Wait.. he's still alive?");

        yield return new WaitForSecondsRealtime(1.2f);
        chatRoller.AddChatMessage("loeq", "This snake is smarter than his grandma.");

        yield return new WaitForSecondsRealtime(1.5f);
        NextCutscene();
    }

    private IEnumerator MissionIntroSequence()
    {
        PlayerManager.Instance.ObjectiveHUD.SetActive(true);
        PlayerManager.Instance.DonationHUD.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        ObjectiveManager.Instance.AddObjective(firstObjective.Objective);

        yield return new WaitForSecondsRealtime(2f);
        PlayerManager.Instance.ObjectiveHUD.GetComponentInChildren<UIBlink>().Stop();
        NextCutscene();
    }


}
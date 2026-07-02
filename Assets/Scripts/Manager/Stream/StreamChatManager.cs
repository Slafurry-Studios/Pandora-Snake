using System.Collections;
using UnityEngine;
using Game.UI.HUD;
public class StreamChatManager : Singleton<StreamChatManager>
{
    [SerializeField] private StreamChatDatabase chatDatabase;
    [SerializeField] private float minChatInterval = 0.2f;
    [SerializeField] private float maxChatInterval = 5f;
    [SerializeField] private float burstInterval = 0.3f;

#if UNITY_EDITOR
    [SerializeField] private bool enableDebugKeys = true;
    [SerializeField] private int debugChatCount = 3;
#endif

    public StreamChatRoller chatRoller;

    protected override void Awake()
    {
        base.Awake();
        chatDatabase?.Initialize();
    }

    void OnEnable()
    {
        chatRoller = FindAnyObjectByType<StreamChatRoller>();
        StartCoroutine(RandomChatLoop());
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!enableDebugKeys) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) HandleStreamChat(StreamChatType.DESTROY_BUILDING, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha2)) HandleStreamChat(StreamChatType.KILL_CIVILIAN, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha3)) HandleStreamChat(StreamChatType.KILL_HOSTILES, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha4)) HandleStreamChat(StreamChatType.ADD_SEGMENTS, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha5)) HandleStreamChat(StreamChatType.SNAKE_BUFF, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha6)) HandleStreamChat(StreamChatType.WANTED_LEVEL, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha7)) HandleStreamChat(StreamChatType.SUBSCRIBE_TRESHOLD, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha8)) HandleStreamChat(StreamChatType.BOSS_FIGHT, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha9)) HandleStreamChat(StreamChatType.MISSION_COMPLETE, debugChatCount);
        if (Input.GetKeyDown(KeyCode.Alpha0)) HandleStreamChat(StreamChatType.TAKE_DAMAGE, debugChatCount);
    }
#endif

    private IEnumerator RandomChatLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minChatInterval, maxChatInterval);
            yield return new WaitForSeconds(waitTime);

            HandleStreamChat(StreamChatType.RANDOM_CHAT, 1);
        }
    }

    public void HandleStreamChat(StreamChatType chatType, int chatCount = 3)
    {
        StartCoroutine(SpawnChatBurst(chatType, chatCount));
    }

    private IEnumerator SpawnChatBurst(StreamChatType chatType, int chatCount)
    {
        for (int i = 0; i < chatCount; i++)
        {
            if (chatDatabase == null || chatRoller == null) yield break;

            StreamChatMessage chat = chatDatabase.GetRandomMessage(chatType);
            if (chat != null)
                chatRoller.AddChatMessage(chat.user, chat.message);

            yield return new WaitForSeconds(burstInterval);
        }
    }
}
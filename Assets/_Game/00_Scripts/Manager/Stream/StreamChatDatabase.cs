using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StreamChatDatabase", menuName = "StreamChat/StreamChatDatabase")]
public class StreamChatDatabase : ScriptableObject
{
    [Header("Tiap StreamChatType di-assign file JSON-nya sendiri")]
    public List<StreamChatFileEntry> fileEntries;

    private Dictionary<StreamChatType, List<StreamChatMessage>> _chatMap;

    public void Initialize()
    {
        _chatMap = new Dictionary<StreamChatType, List<StreamChatMessage>>();

        if (fileEntries == null || fileEntries.Count == 0)
        {
            Debug.LogWarning("[StreamChatDatabase] Belum ada file entry yang di-assign.");
            return;
        }

        foreach (var entry in fileEntries)
        {
            if (entry.jsonFile == null)
            {
                Debug.LogWarning($"[StreamChatDatabase] JSON untuk {entry.type} belum di-assign.");
                continue;
            }

            StreamChatMessages parsed = JsonUtility.FromJson<StreamChatMessages>(entry.jsonFile.text);

            if (parsed == null || parsed.messages == null)
            {
                Debug.LogWarning($"[StreamChatDatabase] Gagal parsing JSON untuk {entry.type}.");
                continue;
            }

            if (!_chatMap.ContainsKey(entry.type))
                _chatMap.Add(entry.type, parsed.messages);
            else
                Debug.LogWarning($"[StreamChatDatabase] Duplikat type {entry.type} di list, di-skip.");
        }
    }

    public List<StreamChatMessage> GetMessages(StreamChatType type)
    {
        if (_chatMap == null) Initialize();

        if (_chatMap.TryGetValue(type, out var messages))
            return messages;

        Debug.LogWarning($"[StreamChatDatabase] Tidak ada pesan untuk type: {type}");
        return null;
    }

    public StreamChatMessage GetRandomMessage(StreamChatType type)
    {
        var messages = GetMessages(type);
        if (messages == null || messages.Count == 0) return null;
        return messages[Random.Range(0, messages.Count)];
    }
}
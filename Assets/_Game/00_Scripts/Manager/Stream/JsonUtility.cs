using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StreamChatEntry
{
    public StreamChatType type;
    public List<string> messages;
}

[System.Serializable]
public class StreamChatMessage
{
    public string user;
    public string message;
}

[System.Serializable]
public class StreamChatMessages
{
    public List<StreamChatMessage> messages;
}

[System.Serializable]
public class StreamChatFileEntry
{
    public StreamChatType type;
    public TextAsset jsonFile;
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.HUD
{
    public class StreamChatRoller : MonoBehaviour
    {
        [SerializeField] private GameObject chatHandlerPrefab;
        [SerializeField] private int maxChatCount = 15;

        private readonly Queue<GameObject> _activeChats = new Queue<GameObject>();

        private void OnEnable()
        {
            StreamChatManager.Instance.chatRoller = this;
        }

        public void AddChatMessage(string user, string message)
        {
            GameObject chatHandler = Instantiate(chatHandlerPrefab, transform);
            chatHandler.GetComponent<ChatHandler>().SetChat(user, message);

            _activeChats.Enqueue(chatHandler);

            if (_activeChats.Count > maxChatCount)
            {
                GameObject oldest = _activeChats.Dequeue();
                Destroy(oldest);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());

        }
    }
}
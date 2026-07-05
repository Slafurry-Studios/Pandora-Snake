using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game.UI.HUD
{
    public class ChatHandler : MonoBehaviour
    {
        private TextMeshProUGUI comments;

        private void Awake()
        {
            comments = transform.Find("Comments").GetComponent<TextMeshProUGUI>();

        }

        public void SetChat(string user, string comments)
        {
            this.comments.text = user + ": " + comments;
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
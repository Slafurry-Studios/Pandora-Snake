using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
{
    public class PlayerCollection : MonoBehaviour
    {
        [Header("Collection Stats")]
        [SerializeField] private int orbsCollected = 0;
        public int GetOrbs() => orbsCollected;

    }
}

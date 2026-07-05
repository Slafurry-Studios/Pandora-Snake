using UnityEngine;


[System.Serializable]
public class SFXEffect
{
    public string groupID;
    public AudioClip[] clips;
    [Range(0f, 10f)]
    public float volume = 1f;
    public int maxSimultaneous = 3;
}

namespace Slafurry.System.Audio
{

    [CreateAssetMenu(fileName = "SFX Category", menuName = "Game/Audio/SFX/Category")]
    public class SFXCategory : ScriptableObject
    {
        public string categoryName;
        public SFXEffect[] effects;
        public int poolSize = 8;
    }
}
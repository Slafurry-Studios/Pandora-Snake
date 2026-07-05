using UnityEngine;
using UnityEngine.UI;
public class PlayerBuffHUD : MonoBehaviour
{
    [SerializeField] private Sprite[] buffSprites;
    [SerializeField] private Image buffImage;

    private int currentBuff;

    public void AddBuff()
    {
        currentBuff += 1;

        if (buffSprites != null && currentBuff >= 0 && currentBuff < buffSprites.Length && buffSprites[currentBuff] != null)
        {
            buffImage.sprite = buffSprites[currentBuff];
        }
    }
}
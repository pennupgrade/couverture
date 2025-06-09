using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour
{
    [SerializeField] private AchievementData achievementData;
    [SerializeField] private Image maskImage;
    [SerializeField] private Image medalImage;

    private void Awake()
    {
        if (achievementData == null) return;

        SetMedalTexture(achievementData.medal);  
    }

    public void SetMedalTexture(Sprite sprite)
    {
        if (sprite == null) return;

        maskImage.sprite = sprite;
        medalImage.sprite = sprite;
    }
}

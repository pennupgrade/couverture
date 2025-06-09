using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour
{
    [SerializeField] private AchievementData achievementData;
    [SerializeField] private Image maskImage;
    [SerializeField] private Image medalImage;

    private void Start()
    {
        SetMedalTexture(achievementData.medal);
    }

    public void SetMedalTexture(Sprite sprite)
    {
        if (sprite == null) return;

        maskImage.sprite = sprite;
        medalImage.sprite = sprite;
    }

    public void CheckToEnable(HashSet<StaticSaveStateManager.Achievement> unlockedAchievements)
    {
        if (unlockedAchievements.Contains(achievementData.associatedEnum))
        {
            medalImage.gameObject.SetActive(true);
        }
    }
}

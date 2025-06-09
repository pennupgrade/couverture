using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour
{
    [SerializeField] private AchievementData achievementData;
    [SerializeField] private Image maskImage;
    [SerializeField] private Image medalImage;
    [SerializeField] private MedalPointerHandler handler;
    [SerializeField] private RectTransform rt;
    [SerializeField] private float enterTime;
    [SerializeField] private float exitTime;

    private readonly static float SCALE_FACTOR = 1.3f;

    private void Start()
    {
        SetMedalTexture(achievementData.medal);

        handler.HandlePointerClick = () => Debug.Log("clicked!");

        handler.HandlePointerEnter = () =>
        {
            LeanTween.cancel(rt);
            LeanTween.scale(rt, new Vector3(SCALE_FACTOR, SCALE_FACTOR, SCALE_FACTOR), enterTime).setEaseOutExpo();
        };

        handler.HandlePointerExit = () =>
        {
            LeanTween.cancel(rt);
            LeanTween.scale(rt, Vector3.one, exitTime).setEaseInOutExpo();
        };

        rt.Rotate(Vector3.forward, Random.Range(-6f, 6f));
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

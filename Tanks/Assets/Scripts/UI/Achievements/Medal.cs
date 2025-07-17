// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

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
    private AchievementsUI achievementUI;

    private void Start()
    {
        // Don't run for the big medal in the Medal Detail UI
        if (handler == null || achievementData == null) return;

        rt.Rotate(Vector3.forward, Random.Range(-6f, 6f));
        SetMedalTexture(achievementData.medal);

        handler.HandlePointerClick = () => achievementUI.OpenMedalDetailUI(achievementData, rt);

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
    }

    public void SetMedalTexture(Sprite sprite)
    {
        if (sprite == null) return;

        maskImage.sprite = sprite;
        medalImage.sprite = sprite;
    }

    public void UnlockMedal() => medalImage.gameObject.SetActive(true);
    public void LockMedal() => medalImage.gameObject.SetActive(false);

    public void Init(HashSet<StaticSaveStateManager.Achievement> unlockedAchievements, AchievementsUI achievementUI)
    {
        this.achievementUI = achievementUI;

        if (unlockedAchievements.Contains(achievementData.associatedEnum))
        {
            UnlockMedal();
        }
    }
}

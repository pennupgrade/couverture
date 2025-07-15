// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private RectTransform frame;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text subtitle;

    [Header("Grids")]
    [SerializeField] private GameObject campaignGridObj;
    [SerializeField] private GameObject classicGridObj;

    [Header("Medal Detail UI")]
    [SerializeField] private CanvasGroup secondOverlay;
    [SerializeField] private RectTransform bigMedalRt;
    [SerializeField] private Button medalDetailButton;
    [SerializeField] private RectTransform medalDetailPanelRt;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;

    private HashSet<StaticSaveStateManager.Achievement> cachedUnlockedAchievements;

    private void OnEnable()
    {
        closeButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
        }, 0f, 1f, 0.15f);

        LeanTween.moveY(frame, -1500f, 0);
        LeanTween.moveY(frame, 0f, 0.5f).setEaseOutExpo().setOnComplete(() => closeButton.interactable = true);
    }

    private void Start()
    {
        cachedUnlockedAchievements = SaveStateManagerGameObject.GetAchievements();

        var unlocked = cachedUnlockedAchievements.Count;
        var total = Enum.GetValues(typeof(StaticSaveStateManager.Achievement)).Length;
        var percentage = Mathf.RoundToInt(unlocked / (float)total * 100);

        subtitle.text = $"<b>{unlocked}/{total} ({percentage}%)</b> unlocked. Click a medal to learn more!";

        foreach (Transform medalTransform in campaignGridObj.transform)
        {
            var medalObj = medalTransform.gameObject;
            var medal = medalObj.GetComponent<Medal>();
            medal.Init(cachedUnlockedAchievements, this);
        }

        foreach (Transform medalTransform in classicGridObj.transform)
        {
            var medalObj = medalTransform.gameObject;
            var medal = medalObj.GetComponent<Medal>();
            medal.Init(cachedUnlockedAchievements, this);
        }
    }

    public void Close()
    {
        if (IsAnimating)
        {
            return;
        }

        StartCoroutine(_Close());
    }

    private IEnumerator _Close()
    {
        closeButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
        }, 1f, 0f, 0.15f);

        LeanTween.moveY(frame, -1500f, 0.2f).setEaseInExpo();

        yield return new WaitWhile(() => IsAnimating);

        gameObject.SetActive(false);
    }

    public bool IsAnimating => LeanTween.isTweening(overlay.gameObject) || LeanTween.isTweening(frame) || LeanTween.isTweening(secondOverlay.gameObject) || LeanTween.isTweening(bigMedalRt) || LeanTween.isTweening(bigMedalRt.gameObject) || LeanTween.isTweening(medalDetailPanelRt);

    private void UpdateMedalDetailUI(AchievementData achievement, RectTransform sourceRt)
    {
        var originalWorldPos = sourceRt.position;
        var newLocalPos = bigMedalRt.parent.transform.InverseTransformPoint(originalWorldPos);

        bigMedalRt.localPosition = newLocalPos;
        bigMedalRt.localScale = sourceRt.localScale * 0.5f; // Big medal scale is half of everything
        bigMedalRt.localRotation = sourceRt.localRotation;

        description.text = achievement.title;

        var bigMedal = bigMedalRt.gameObject.GetComponent<Medal>();
        bigMedal.SetMedalTexture(achievement.medal);

        if (cachedUnlockedAchievements.Contains(achievement.associatedEnum))
        {
            title.text = achievement.title;
            bigMedal.UnlockMedal();
        }
        else
        {
            title.text = "??? (Locked)";
            bigMedal.LockMedal();
        }
    }

    public void OpenMedalDetailUI(AchievementData achievement, RectTransform sourceRt)
    {
        if (IsAnimating) return;

        UpdateMedalDetailUI(achievement, sourceRt);
        OpenMedalDetailUI();
    }

    private void OpenMedalDetailUI()
    {
        medalDetailButton.interactable = false;

        LeanTween.value(secondOverlay.gameObject, value =>
        {
            secondOverlay.alpha = value;
        }, 0f, 1f, 0.1f);
        secondOverlay.gameObject.SetActive(true);

        var originalZAngle = bigMedalRt.localEulerAngles.z;
        LeanTween.value(bigMedalRt.gameObject, value =>
        {
            bigMedalRt.localEulerAngles = new Vector3(0f, value, originalZAngle);
        }, 0f, 359f, 1f).setEaseOutExpo();
        bigMedalRt.localEulerAngles = new Vector3(0f, 0f, originalZAngle);

        LeanTween.value(bigMedalRt.gameObject, value =>
        {
            bigMedalRt.localScale = value;
        }, bigMedalRt.localScale, new Vector3(1.4f, 1.4f, 1.4f), 1.1f).setEaseOutExpo();

        LeanTween.move(bigMedalRt, new Vector3(516f, -540f, 0f), 1.1f).setEaseOutExpo();
        LeanTween.moveY(medalDetailPanelRt, 0f, 0.8f).setEaseOutExpo().setOnComplete(() => medalDetailButton.interactable = true);

        bigMedalRt.gameObject.SetActive(true);
    }

    public void CloseMedalDetailUI()
    {
        if (IsAnimating) return;

        medalDetailButton.interactable = false;

        LeanTween.value(secondOverlay.gameObject, value =>
        {
            secondOverlay.alpha = value;
        }, 1f, 0f, 0.1f).setOnComplete(() => secondOverlay.gameObject.SetActive(false));

        LeanTween.moveY(medalDetailPanelRt, -1000f, 0.25f).setEaseInExpo().setOnComplete(() => { medalDetailButton.interactable = true; });
        LeanTween.moveY(bigMedalRt, -1540f, 0.25f).setEaseInExpo().setOnComplete(() => bigMedalRt.gameObject.SetActive(false));
    }
}

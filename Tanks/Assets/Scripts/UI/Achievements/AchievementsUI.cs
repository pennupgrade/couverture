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
    [SerializeField] private GameObject gridObj;

    [Header("Medal Detail UI")]
    [SerializeField] private CanvasGroup secondOverlay;
    [SerializeField] private RectTransform bigMedalRt;

    private HashSet<StaticSaveStateManager.Achievement> cachedUnlockedAchievements;

    private void OnEnable()
    {
        closeButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
        }, 0f, 1f, 0.15f);

        LeanTween.moveY(frame, -1500f, 0);
        LeanTween.moveY(frame, 0f, 0.5f).setEaseOutExpo();

        closeButton.interactable = true;
    }

    private void Start()
    {
        cachedUnlockedAchievements = SaveStateManagerGameObject.GetAchievements();

        var unlocked = cachedUnlockedAchievements.Count;
        var total = Enum.GetValues(typeof(StaticSaveStateManager.Achievement)).Length;
        var percentage = Mathf.RoundToInt(unlocked / (float)total * 100);

        subtitle.text = $"<b>{unlocked}/{total} ({percentage}%)</b> unlocked. Click a medal to learn more!";

        foreach (Transform medalTransform in gridObj.transform)
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

    public bool IsAnimating => LeanTween.isTweening(overlay.gameObject) || LeanTween.isTweening(frame) || LeanTween.isTweening(secondOverlay.gameObject) || LeanTween.isTweening(bigMedalRt);

    private void UpdateMedalDetailUI(AchievementData achievement, RectTransform sourceRt)
    {
        var originalWorldPos = sourceRt.position;
        var newLocalPos = bigMedalRt.parent.transform.InverseTransformPoint(originalWorldPos);

        bigMedalRt.localPosition = newLocalPos;
        bigMedalRt.localScale = sourceRt.localScale * 0.5f; // Big medal scale is half of everything
        bigMedalRt.localRotation = sourceRt.localRotation;

        var bigMedal = bigMedalRt.gameObject.GetComponent<Medal>();
        bigMedal.SetMedalTexture(achievement.medal);

        if (cachedUnlockedAchievements.Contains(achievement.associatedEnum))
        {
            bigMedal.UnlockMedal();
        }
        else
        {
            bigMedal.LockMedal();
        }
    }

    public void OpenMedalDetailUI(AchievementData achievement, RectTransform sourceRt)
    {
        UpdateMedalDetailUI(achievement, sourceRt);
        OpenMedalDetailUI();
    }

    private void OpenMedalDetailUI()
    {
        closeButton.interactable = false;

        LeanTween.value(secondOverlay.gameObject, value =>
        {
            secondOverlay.alpha = value;
        }, 0f, 1f, 0.1f);
        secondOverlay.gameObject.SetActive(true);

        LeanTween.rotate(bigMedalRt, 0f, 1f).setEaseOutExpo();

        bigMedalRt.gameObject.SetActive(true);

        // todo: remove unused achievements

        closeButton.interactable = true;
    }
}

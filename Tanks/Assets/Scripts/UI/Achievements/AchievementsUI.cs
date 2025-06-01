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

    private HashSet<StaticSaveStateManager.Achievement> cachedAchievements;

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
        cachedAchievements = SaveStateManagerGameObject.GetAchievements();

        var unlocked = cachedAchievements.Count;
        var total = Enum.GetValues(typeof(StaticSaveStateManager.Achievement)).Length;
        var percentage = Mathf.RoundToInt(unlocked / (float)total);

        subtitle.text = $"<b>{unlocked}/{total} ({percentage}%)</b> unlocked";
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

    public bool IsAnimating => LeanTween.isTweening(overlay.gameObject) || LeanTween.isTweening(frame);
}

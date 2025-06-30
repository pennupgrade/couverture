using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlay;

    [Header("Panel")]
    [SerializeField] private RectTransform panelRt;
    [SerializeField] private CanvasGroup panelCg;

    [Header("Music")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicPillText;

    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxPillText;

    [Header("Buttons")]
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    public bool IsAnimating => LeanTween.isTweening(overlay.gameObject) || LeanTween.isTweening(panelRt);

    private void Start()
    {
        overlay.alpha = 0f;

        musicSlider.onValueChanged.AddListener((value) =>
        {
            var sfx = SaveStateManagerGameObject.GetSFXVolume();
            SaveStateManagerGameObject.SetVolume(sfx, value / 100f);
            musicPillText.text = Math.Truncate(value).ToString();
        });

        // TODO: sfx onvaluechanged
    }

    private void OnEnable()
    {
        closeButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
            panelCg.alpha = value;
        }, 0f, 1f, 0.35f).setOnComplete(() => overlay.alpha = 1f).setEaseOutExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, 0f).setIgnoreTimeScale(true);
        LeanTween.moveY(panelRt, 0f, 0.35f).setEaseOutExpo().setOnComplete(() => closeButton.interactable = true).setIgnoreTimeScale(true);

        var music = SaveStateManagerGameObject.GetMusicVolume();
        var sfx = SaveStateManagerGameObject.GetSFXVolume();

        var musicValue = music * 100f;
        musicSlider.value = musicValue;
        musicPillText.text = Math.Truncate(musicValue).ToString();

        // TODO: sfx onenable
    }

    public void CloseOptionsMenu()
    {
        closeButton.interactable = false;
        openButton.interactable = false;

        LeanTween.value(overlay.gameObject, value =>
        {
            overlay.alpha = value;
            panelCg.alpha = value;
        }, 1f, 0f, 0.35f).setOnComplete(() => overlay.alpha = 0f).setEaseInExpo().setIgnoreTimeScale(true);

        LeanTween.moveY(panelRt, -200f, 0.35f).setEaseInExpo().setOnComplete(() =>
        {
            closeButton.interactable = true;
            openButton.interactable = true;

            gameObject.SetActive(false);
        }).setIgnoreTimeScale(true);

        SaveStateManagerGameObject.SetVolume(sfxSlider.value / 100f, musicSlider.value / 100f);
    }
}

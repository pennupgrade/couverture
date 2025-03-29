using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectDisplay : MonoBehaviour
{
    [SerializeField] private GameObject lockedGraphic;
    [SerializeField] private Image mainImage;
    [SerializeField] private TMP_Text lockedText;
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform rect;

    private bool locked;

    public void LockLevel(bool levelLocked)
    {
        locked = levelLocked;
        lockedGraphic.SetActive(locked);
        if (locked)
            mainImage.color = Color.clear;
        lockedText.gameObject.SetActive(locked);
        mainText.gameObject.SetActive(!locked);
        button.interactable = !locked;
    }
    
    public void HoverButton()
    {
        if (locked)
            return;
        LeanTween.cancel(gameObject);
        LeanTween.scale(rect, 1.15f * Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
    }

    public void EndHoverButton()
    {
        if (locked)
            return;
        LeanTween.cancel(gameObject);
        LeanTween.scale(rect, 1f * Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class GameplayHUDManager : MonoBehaviour
{
    public RectTransform abilityIcon;
    public RectTransform readyTag;
    public Image abilityBarFill;
    private bool abilityEnabled;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = abilityIcon.anchoredPosition;
        EnableAbilityBar(false);
    }

    public void AbilityBarIsCasting()
    {
        abilityBarFill.fillAmount = 0;
        readyTag.gameObject.SetActive(false);
    }

    public void StartFillAbilityBar(float reloadTime)
    {
        abilityBarFill.fillAmount = 0;
        readyTag.gameObject.SetActive(false);
        
        LeanTween.cancel(gameObject);
        abilityIcon.anchoredPosition = startPosition;
        LeanTween.moveY(abilityIcon, startPosition.y - 30f, 0.12f).setEaseOutCubic().setIgnoreTimeScale(true);
        LeanTween.moveY(abilityIcon, startPosition.y, 0.18f).setEaseOutQuart().setIgnoreTimeScale(true).setDelay(0.12f);
        
        StartCoroutine(FillAbilityBar(reloadTime));
    }

    private IEnumerator FillAbilityBar(float reloadTime)
    {
        float elapsed = 0f;
        abilityBarFill.fillAmount = 0f;

        while (elapsed < reloadTime)
        {
            yield return new WaitForSeconds(0.05f);
            elapsed += 0.05f;
            abilityBarFill.fillAmount = elapsed / reloadTime;
        }

        abilityBarFill.fillAmount = 1f;
        readyTag.gameObject.SetActive(true);
        StartReadyTagAnim();
    }
    
    private void StartReadyTagAnim()
    {
        LeanTween.cancel(readyTag.gameObject);
        LeanTween.moveY(readyTag, 16f, 0.25f).setEase(LeanTweenType.easeInQuart).setIgnoreTimeScale(true)
            .setOnComplete(ReadyTagBounceDown);
    }
    
    private void ReadyTagBounceUp()
    {
        LeanTween.moveY(readyTag, 16f, 0.25f).setEase(LeanTweenType.easeInQuart).setIgnoreTimeScale(true).setOnComplete(ReadyTagBounceDown).setDelay(0.25f);
    }

    private void ReadyTagBounceDown()
    {
        LeanTween.moveY(readyTag, -4f, 0.25f).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true).setOnComplete(ReadyTagBounceUp);
    }

    public void EnableAbilityBar(bool enable)
    {
        if (enable)
        {
            abilityEnabled = true;
            abilityIcon.gameObject.SetActive(true);
            LeanTween.moveY(abilityIcon, -100f, 0f);
            LeanTween.moveY(abilityIcon, 45f, 0.5f).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true).setDelay(0.2f);
        }
        else
        {
            abilityEnabled = false;
            abilityIcon.gameObject.SetActive(false);
        }
    }
}

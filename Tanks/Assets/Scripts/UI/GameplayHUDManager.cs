using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHUDManager : MonoBehaviour
{
    public RectTransform abilityIcon;
    public RectTransform readyTag;
    public Image rocketAbilityIcon;
    public Image bubbleAbilityIcon;
    [HideInInspector] public Image abilityBarFill;

    private Vector3 abilityIconStartPosition;

    private void Start()
    {
        abilityIconStartPosition = abilityIcon.anchoredPosition;
    }

    public void ActivateAbilityIcon()
    {
        SaveStateManager.CharacterOption characterOption = SaveStateManagerGameObject.GetCurrentCharacter();
        if (characterOption == SaveStateManager.CharacterOption.DEFAULT_CAT)
        {
            EnableAbilityBar(false);
        }
        else
        {
            SetAbilityIcon(characterOption == SaveStateManager.CharacterOption.BUBBLE_CAT);
            EnableAbilityBar(true);
        }
    }

    private void SetAbilityIcon(bool bubbleCat)
    {
        abilityBarFill = bubbleCat ? bubbleAbilityIcon : rocketAbilityIcon;
        rocketAbilityIcon.gameObject.SetActive(!bubbleCat);
        bubbleAbilityIcon.gameObject.SetActive(bubbleCat);
    }

    public void AbilityBarIsCasting(float stayTime, Character tank) {
        abilityBarFill.fillAmount = 0;
        readyTag.gameObject.SetActive(false);
        StartCoroutine(DrainAbilityBar(stayTime, tank));
    }

    public void StartFillAbilityBar(float reloadTime) {
        Debug.Log("Here we are filling the ability bar: " + reloadTime);
        abilityBarFill.fillAmount = 0;
        readyTag.gameObject.SetActive(false);

        LeanTween.cancel(gameObject);
        abilityIcon.anchoredPosition = abilityIconStartPosition;
        LeanTween.moveY(abilityIcon, abilityIconStartPosition.y - 30f, 0.12f).setEaseOutCubic().setIgnoreTimeScale(true);
        LeanTween.moveY(abilityIcon, abilityIconStartPosition.y, 0.18f).setEaseOutQuart().setIgnoreTimeScale(true).setDelay(0.12f);

        StartCoroutine(FillAbilityBar(reloadTime));
    }

    private IEnumerator FillAbilityBar(float reloadTime) {
        var elapsed = 0f;
        abilityBarFill.fillAmount = 0f;

        while (elapsed < reloadTime) {
            yield return new WaitForSeconds(0.05f);
            elapsed += 0.05f;
            abilityBarFill.fillAmount = elapsed / reloadTime;
        }

        abilityBarFill.fillAmount = 1f;
        readyTag.gameObject.SetActive(true);
        StartReadyTagAnim();
    }

    private IEnumerator DrainAbilityBar(float reloadTime, Character tank) {
        var elapsed = 0f;
        readyTag.gameObject.SetActive(true);
        readyTag.GetComponentInChildren<TMP_Text>().text = "Active!";
        abilityBarFill.fillAmount = 1f;

        while (elapsed < reloadTime) {
            if (tank.GetType() == typeof(BubbleChar)) {
                if (((BubbleChar)tank).active == false) {
                    break;
                }
            }

            yield return new WaitForSeconds(0.05f);
            elapsed += 0.05f;
            abilityBarFill.fillAmount = 1 - elapsed / reloadTime;
        }

        abilityBarFill.fillAmount = 0f;
        readyTag.GetComponentInChildren<TMP_Text>().text = "Ready!";
        readyTag.gameObject.SetActive(false);
        StartReadyTagAnim();
    }

    private void StartReadyTagAnim() {
        LeanTween.cancel(readyTag.gameObject);
        LeanTween.moveY(readyTag, 16f, 0.25f).setEase(LeanTweenType.easeInQuart).setIgnoreTimeScale(true)
                 .setOnComplete(ReadyTagBounceDown);
    }

    private void ReadyTagBounceUp() {
        LeanTween.moveY(readyTag, 16f, 0.25f).setEase(LeanTweenType.easeInQuart).setIgnoreTimeScale(true)
                 .setOnComplete(ReadyTagBounceDown).setDelay(0.25f);
    }

    private void ReadyTagBounceDown() {
        LeanTween.moveY(readyTag, -4f, 0.25f).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true)
                 .setOnComplete(ReadyTagBounceUp);
    }

    private void EnableAbilityBar(bool enable) {
        if (enable) {
            abilityIcon.gameObject.SetActive(false);
            LeanTween.moveY(abilityIcon, -250f, 0f);
            StartCoroutine(EnableAbilityIconAnimation());
        }
        else {
            abilityIcon.gameObject.SetActive(false);
        }
    }

    private IEnumerator EnableAbilityIconAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        LeanTween.moveY(abilityIcon, 45f, 1f).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true);
        abilityIcon.gameObject.SetActive(true);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayHUDManager : MonoBehaviour
{
    public TMP_Text ready_Text;
    public Image abilityBar_Fill;

    public void startFillAbilityBar(float reloadTime)
    {
        abilityBar_Fill.fillAmount = 0;
        ready_Text.text = "Recharging..";
        ready_Text.fontSize = 12;
        StartCoroutine("FillAbilityBar", reloadTime);
    }

    private IEnumerator FillAbilityBar(float reloadTime)
    {
        Debug.Log("Reload Time: " + reloadTime);
        float elapsed = 0f;
        abilityBar_Fill.fillAmount = 0f;

        while (elapsed < reloadTime)
        {
            yield return new WaitForSeconds(0.05f);
            elapsed += 0.05f;
            abilityBar_Fill.fillAmount = elapsed / reloadTime;
            Debug.Log(abilityBar_Fill.fillAmount);
        }

        abilityBar_Fill.fillAmount = 1f;
        ready_Text.fontSize = 16;
        ready_Text.text = "Ready!";
    }
}

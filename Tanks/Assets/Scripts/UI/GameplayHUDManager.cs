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
        ready_Text.text = "Recharging...";
        StartCoroutine("fillAbilityBar", reloadTime);
    }

    private IEnumerator fillAbilityBar(float reloadTime)
    {
        float fillConstant = 1 / reloadTime * 0.05f;
        while (reloadTime > 0)
        {
            yield return new WaitForSeconds(0.05f);
            reloadTime -= 0.05f;
            abilityBar_Fill.fillAmount += fillConstant;
        }
        ready_Text.text = "Ready!";
    }
}

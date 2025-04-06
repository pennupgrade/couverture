using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedEnemy : Enemy
{
    public GameObject shield;
    private Material shieldMat;
    [Tooltip("If true, enemy will start with shield generator")]
    public bool shieldEnabled;
    protected bool invincible;
    private bool shieldActivated;
    protected void shieldSetup() {
        shieldMat = shield.GetComponent<MeshRenderer>().material;
    }
    public override void takeDamage(int dmg) {
        if (shieldActivated && invincible) {
            return;
        }
        if (shieldActivated) {
            StartCoroutine(deactivateShield());
            bubbleSound();
            return;
        }
        health -= dmg;
        damageFlash.CallDamageFlash(this);
        if (health <= 0 && !isDead) {
            die();
        }
    }
    protected IEnumerator activateShield() {
        if (shield == null) yield break;
        shieldMat.SetFloat("_Dissolve", 0);

        Vector3 origScale = shield.transform.localScale;
        float i = 0.1f;
        while (i < 1) {
            float f = Mathf.SmoothStep(0.2f, 1.2f, i);
            shield.transform.localScale = f * origScale;
            i += 4 * Time.deltaTime;
            yield return null;
        }
        i = 0.1f;
        while (i < 1) {
            float f = Mathf.SmoothStep(1.2f, 1, i);
            shield.transform.localScale = f * origScale;
            i += 20 * Time.deltaTime;
            yield return null;
        }
        shield.transform.localScale = origScale;
        shieldActivated = true;
    }
    private IEnumerator deactivateShield() {
        shieldActivated = false;
        float i = 0;
        while (i < 1) {
            i += 0.12f;
            shieldMat.SetFloat("_Dissolve", i);
            yield return new WaitForSeconds(0.05f);
        }
        shieldMat.SetFloat("_Dissolve", 1);
        if (shieldEnabled) {
            StartCoroutine(reactivateShield());
        }
    }
    protected virtual IEnumerator reactivateShield() {
        yield return new WaitForSeconds(20);
        StartCoroutine(activateShield());
    }

    public void setInvincible(bool inv) {
        invincible = inv;
        if (inv) {
            if (!shieldEnabled) {
                StartCoroutine(activateShield());
            }
        }
    }
    public bool getShieldActivated() {
        return shieldActivated;
    }

}

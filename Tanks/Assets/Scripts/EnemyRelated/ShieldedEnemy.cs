using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedEnemy : Enemy
{
    public GameObject shield;
    private Material shieldMat;
    public bool shieldEnabled;
    private bool shieldActivated;
    protected void shieldSetup() {
        shieldMat = shield.GetComponent<MeshRenderer>().material;
    }
    public override void takeDamage(int dmg) {
        if (shieldActivated) {
            StartCoroutine(deactivateShield());
            return;
        }
        health -= dmg;
        damageFlash.CallDamageFlash(this);
        if (health <= 0 && !isDead) {
            die();
        }
    }
    protected IEnumerator activateShield() {
        float i = 1;
        while (i > 0) {
            i -= 0.05f;
            shieldMat.SetFloat("_Dissolve", i);
            yield return new WaitForSeconds(0.06f);
        }
        shieldActivated = true;
        shieldMat.SetFloat("_Dissolve", 0);
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
        StartCoroutine(reactivateShield());
    }
    protected virtual IEnumerator reactivateShield() {
        yield return new WaitForSeconds(16);
        StartCoroutine(activateShield());
    }

}

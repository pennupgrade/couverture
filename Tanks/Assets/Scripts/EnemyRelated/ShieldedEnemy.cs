using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedEnemy : Enemy
{
    public Material shield;
    public bool shieldEnabled;
    private bool shieldActivated;
    public override void takeDamage(int dmg) {
        if (shieldEnabled) {
            StartCoroutine(deactivateShield());
            return;
        }
        health -= dmg;
        damageFlash.CallDamageFlash(this);
        if (health <= 0 && !isDead) {
            die();
        }
    }
    private IEnumerator activateShield() {
        shieldActivated = true;

        yield return new WaitForSeconds(1);
    }
    private IEnumerator deactivateShield() {
        shieldActivated = false;
        StartCoroutine(reactivateShield());

        yield return new WaitForSeconds(1);
    }
    protected virtual IEnumerator reactivateShield() {
        yield return new WaitForSeconds(16);
        StartCoroutine(activateShield());
    }

}

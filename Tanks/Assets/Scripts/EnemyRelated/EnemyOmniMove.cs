using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOmniMove : ShieldedEnemy
{
    public bool backwards, accel;
    private GameObject bullet;
    


    //----------------------bullet dodging---------------------------------------------
    protected bool detectBullet(float r) {
        Collider[] hitColliders = new Collider[8];
        Physics.OverlapSphereNonAlloc(transform.position, r, hitColliders, 1 << 7);
        float minDist = 8;
        bool flag = false;
        foreach (var c in hitColliders) {
            if (c != null &&
                    Vector3.Dot(c.transform.forward, (transform.position - c.transform.position).normalized) > 0.1f &&
                    Vector3.Distance(c.transform.position, transform.position) < minDist) {
                minDist = Vector3.Distance(c.transform.position, transform.position);
                bullet = c.gameObject;
                flag = true;
            }
        }
        return flag;
    }

    protected void dodge() {
        if (bullet == null) return;
        Vector3 dir = (transform.position - bullet.transform.position).normalized; 
        if (Mathf.Abs(Vector3.Dot(bullet.transform.right, dir)) > 0.3f) {
            turnTowardsVectorOmni(dir);
        } else {
            turnTowardsVectorOmni((Vector3.Cross((numBullets > 1) ? dir : -dir, Vector3.up) + dir).normalized);
        }
    }

    protected void turnTowardsVectorOmni(Vector3 v) {
        backwards = Vector3.Dot(transform.right, v) < 0;
        float dir = Vector3.Dot(transform.forward, v);
        if ((dir > 0.03f && !backwards) || (dir < -0.03f && backwards)) {
            if (cTurnSpeed > 0) {
                cTurnSpeed -= 2 * 400 * Time.fixedDeltaTime;
            } else {
                cTurnSpeed = Mathf.Max(-turnSpeed, cTurnSpeed - 400 * Time.fixedDeltaTime);
            }
        } else if ((dir < -0.03f && !backwards) || (dir > 0.03f && backwards)) {
            if (cTurnSpeed < 0) {
                cTurnSpeed += 2 * 400 * Time.fixedDeltaTime;
            } else {
                cTurnSpeed = Mathf.Min(turnSpeed, cTurnSpeed + 400 * Time.fixedDeltaTime);
            }
        } else {
            cTurnSpeed = 0;
        }
    }
}

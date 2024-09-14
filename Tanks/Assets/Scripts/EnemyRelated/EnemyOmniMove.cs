using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOmniMove : ShieldedEnemy
{
    [HideInInspector] public bool backwards, accel;
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
        float mag = (transform.position - bullet.transform.position).magnitude;
        if (mag < 0.4f || Mathf.Abs(Vector3.Dot(bullet.transform.right, dir)) > 0.35f) {
            turnTowardsVectorOmni(dir);
        } else {
            if (Vector3.Dot(dir, Vector3.Cross(bullet.transform.forward, Vector3.up)) > 0) {
                turnTowardsVectorOmni((Vector3.Cross(bullet.transform.forward, Vector3.up) + dir).normalized);
            } else {
                turnTowardsVectorOmni((Vector3.Cross(Vector3.up, bullet.transform.forward) + dir).normalized);
            }
            
        }
    }

    protected void turnTowardsVectorOmni(Vector3 v) {
        //Debug.DrawRay(transform.position, 3* v, Color.red);
        backwards = Vector3.Dot(transform.forward, v) < 0;
        float dir = Vector3.Dot(-transform.right, v);
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

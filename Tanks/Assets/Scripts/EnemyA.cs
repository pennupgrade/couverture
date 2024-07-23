using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy: Gun Emplacement
public class EnemyA : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        activeRadius = 7;
        state = EnemyState.Idle;
        rotSpeed = 56;
        dispersion = 24;
        reload = 5;
        bulletSpeed = 3;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(idleTurn());
    }

    // Update is called once per frame
    void Update()
    {
        if (checkTimer < 0.01f) {
            if (playerCheck()) {
                state = EnemyState.Alert;
                checkTimer = 8;
            } else {
                state = EnemyState.Idle;
                checkTimer = 1.5f + Random.value;
            }
        }

        if (state == EnemyState.Alert && reloadTimer < 0.01f) {
            hasLineOfSight = lineOfSightCheck();
            aimReady = isAimed();
            if (hasLineOfSight && aimReady) {
                fire();
                reloadTimer = reload;
            } else {
                reloadTimer = 0.25f;
            }
        }

        reloadTimer = TimerF(reloadTimer);
        checkTimer = TimerF(checkTimer);
    }
    void FixedUpdate() {
        if (state == EnemyState.Alert) {
            if (MyMath.InterceptDirection(playerRB.position, rb.position, playerRB.velocity, bulletSpeed, out Vector3 result)){
                    TargetDir = result;
            } else TargetDir = (playerRB.position - rb.position).normalized;
            if (Vector3.Dot(gun.transform.forward, TargetDir) > 0){
                cTurn = -rotSpeed;
            } else cTurn = rotSpeed;
        }
        gun.transform.eulerAngles += cTurn * Time.fixedDeltaTime * Vector3.up; 
    }
}

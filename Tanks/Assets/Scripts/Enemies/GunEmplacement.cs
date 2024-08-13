using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEmplacement : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        health = 300;
        activeRadius = 6;
        state = EnemyState.Idle;
        rotSpeed = 72;
        dispersion = 24;
        reload = 3;
        bulletSpeed = 2.5f;
        leadChance = 0.25f;
        rb = GetComponent<Rigidbody>();
        //coroutine for idle turret turning
        StartCoroutine(idleTurn());
    }

    // Update is called once per frame
    void Update()
    {
        // changes between states depending on whether player is detected
        if (checkTimer < 0.01f) {
            if (checkIfPlayerDetected()) {
                state = EnemyState.Alert;
                checkTimer = 7;
            } else {
                state = EnemyState.Idle;
                checkTimer = 1 + Random.value;
            }
        }

        // conditions for shooting
        if (state == EnemyState.Alert && reloadTimer < 0.01f) {
            hasLineOfSight = lineOfSightCheck();
            if (hasLineOfSight && isAimed()) {
                fire();
                reloadTimer = reload;
            } else {
                reloadTimer = 0.16f;
            }
        }

        reloadTimer = TimerF(reloadTimer);
        checkTimer = TimerF(checkTimer);
    }
    void FixedUpdate() {
        //turret turning
        if (state == EnemyState.Alert) {
            turnTurret();
        }
        gun.transform.eulerAngles += cTurn * Time.fixedDeltaTime * Vector3.up; 
    }
}

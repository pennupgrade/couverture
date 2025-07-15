// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry2 : Enemy
{
    [HideInInspector] public bool pauseRot;
    void Awake() {
        enemyState = new Sentry2_Idle(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 400;
        sightRange = 10;
        gunRange = 9;
        FOV = 1.5f;
        rotSpeed = 70;
        cooldownTime = 0.4f;
        magSize = 3;
        numBullets = magSize;
        reload = 2.5f;
        bulletSpeed = 3.25f;
        leadChance = 0.25f;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        enemyState = enemyState.Patrol(playerPos);
        enemyState = enemyState.RotateTurret(playerPos);
        enemyState = enemyState.Shoot(playerPos);

        if (!pauseRot) {
            gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
        }
    }
}

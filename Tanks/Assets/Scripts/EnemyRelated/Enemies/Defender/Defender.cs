// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : ShieldedEnemy
{
    [HideInInspector] public bool pauseRot;
    void Awake() {
        enemyState = new Defend_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 500;
        sightRange = 15;
        FOV = 1.2f;
        rotSpeed = 45;
        reload = 3;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        if (shieldEnabled) {
            shieldSetup();
            StartCoroutine(activateShield());
        }
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

    protected override IEnumerator reactivateShield() {
        yield return new WaitForSeconds(RoomManager.playerIsRocket ? 7 : 15);
        StartCoroutine(activateShield());
    }
}

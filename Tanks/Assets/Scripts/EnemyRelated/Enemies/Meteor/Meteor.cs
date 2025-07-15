// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Enemy
{
    void Awake() {
        enemyState = new Meteor_Idle(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 500;
        gunRange = 20;
        sightRange = 20;
        FOV = 1.7f;
        rotSpeed = 60;
        reload = 8;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject);
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

        gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
    }

    public bool fireMissile(Vector3 target) {
        if (Mathf.Abs(target.y - transform.position.y) > 0.5f) return false;
        fireSound();
        GameObject missile = Instantiate(bulletPrefab, gunShotPos.position, Quaternion.identity);
        MissileScript missileScript = missile.GetComponent<MissileScript>();
        missileScript.initialize(gunShotPos.position, target);
        return true;
    }

}

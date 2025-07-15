// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : Enemy
{ 
    public bool camoOnStart;
    void Awake() {
        enemyState = new Phantom_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 300;
        gunRange = 8;
        sightRange = 10;
        FOV = 1.3f;
        rotSpeed = 108;
        cooldownTime = 0.7f;
        reload = 4;
        magSize = 4;
        numBullets = magSize;
        bulletSpeed = 3.2f;
        leadChance = 0.3f;
        speed = 1.2f;
        turnSpeed = 200;
        dodgeChance = 0.8f;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        rb = GetComponent<Rigidbody>();
        if (camoOnStart) {
            damageFlash.CallDissolve(this, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;
        enemyState = enemyState.Patrol(playerPos);
        enemyState = enemyState.RotateTurret(playerPos);
        enemyState = enemyState.Shoot(playerPos);

        gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
    }
    void FixedUpdate() {
        agent.nextPosition = transform.position;
        if (isStunned) return;
        //turning
        if (!stopTurns && moveStraightTimer == null) {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= 0.5f * cTurnSpeed * Time.fixedDeltaTime * Vector3.up; 
        }
        //moving
        transform.position += cSpeed * Time.fixedDeltaTime * transform.forward;
    }
    void OnCollisionEnter(Collision collision) {
        if ((collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Tank")
             && cSpeed > 0.01f){
            StartCoroutine(stopMove(0.3f));
        }
    }
    public IEnumerator activateCamo() {
        if (!camoOnStart) {
            yield return new WaitForSeconds(1 + Random.value);
            damageFlash.CallDissolve(this, 2);
            playSound("Ping");
            yield return new WaitForSeconds(2);
        }
        while (true) {
            yield return new WaitForSeconds(2f + 1.2f * Random.value);
            damageFlash.CallInvisFlicker(this, 1.5f);
            playSound("Ping");
        }
    }
    public override void takeDamage(int dmg) {
        health -= dmg;
        if (health <= 0 && !isDead) {
            die();
        } else {
            damageFlash.CallInvisDamage(this, 0.3f);
        }
    }
    public override void alert(bool alertState = false) {}
}

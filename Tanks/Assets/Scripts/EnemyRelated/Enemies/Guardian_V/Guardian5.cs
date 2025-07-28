// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian5 : EnemyOmniMove
{
    void Awake()
    {
        enemyState = new G5_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 300;
        gunRange = 10;
        sightRange = 10;
        FOV = 1.9f;
        rotSpeed = 108;
        cooldownTime = 0.7f;
        reload = 4f;
        magSize = 4;
        numBullets = magSize;
        bulletSpeed = 5.1f;
        leadChance = 0.5f;
        speed = 1.9f;
        turnSpeed = 180;

        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        shieldSetup();
        StartCoroutine(activateShield());
        rb = GetComponent<Rigidbody>();
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
    void FixedUpdate()
    {
        agent.nextPosition = transform.position;
        if (isStunned) return;

        //dodging
        stopTurns = detectBullet(1.9f);
        if (stopTurns)
        {
            dodge();
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            if (!accel)
            {
                alert();
            }
        }
        //turning
        else if (moveStraightTimer == null)
        {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
        }

        //moving
        if (accel && moveStraightTimer == null)
        {
            cSpeed = (backwards ? (Mathf.Max(-speed, cSpeed - 10 * Time.fixedDeltaTime)) :
                                (Mathf.Min(speed, cSpeed + 10 * Time.fixedDeltaTime)));
        }
        transform.position += cSpeed * Time.fixedDeltaTime * transform.forward;
    }
    
    protected override IEnumerator reactivateShield() {
        yield return new WaitForSeconds(RoomManager.playerIsRocket ? 6 : 20);
        StartCoroutine(activateShield());
    }
}

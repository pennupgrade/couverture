using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wasp : PatrollingEnemyOmni
{
    void Awake() {
        enemyState = new Wasp_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 100;
        gunRange = 7;
        sightRange = 8;
        FOV = 1.6f;
        rotSpeed = 108;
        reload = 4.5f;
        bulletSpeed = 3.1f;
        leadChance = 0.25f;
        speed = 2.5f;
        turnSpeed = 180;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
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

        gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
    }
    void FixedUpdate() {
        agent.nextPosition = transform.position;
        if (isStunned) return;

        //turning
        if (moveStraightTimer == null) {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= cTurnSpeed * Time.fixedDeltaTime * Vector3.up; 
        }

        //moving
        if (accel && moveStraightTimer == null) {
            cSpeed = (backwards ? (Mathf.Max(-speed, cSpeed - 18 * Time.fixedDeltaTime)) : 
                                (Mathf.Min(speed, cSpeed + 18 * Time.fixedDeltaTime)));
        }
        transform.position += cSpeed * Time.fixedDeltaTime * transform.forward;
    }
}

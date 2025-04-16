using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian2 : ShieldedEnemy
{
    void Awake() {
        enemyState = new G2_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 500;
        gunRange = 9;
        sightRange = 10;
        FOV = 1.5f;
        rotSpeed = 60;
        cooldownTime = 0.6f;
        reload = 3;
        magSize = 5;
        numBullets = magSize;
        bulletSpeed = 3.2f;
        leadChance = 0.33f;
        speed = 1.4f;
        turnSpeed = 200;
        dodgeChance = 0.85f;
        
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
            StartCoroutine(stopMove(0.4f));
        }
    }
}

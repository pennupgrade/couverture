using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian1 : ShieldedEnemy
{
    void Awake() {
        enemyState = new G1_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 300;
        gunRange = 9;
        sightRange = 10;
        FOV = 0.8f;
        rotSpeed = 90;
        cooldownTime = 0.8f;
        reload = 4;
        magSize = 3;
        numBullets = magSize;
        bulletSpeed = 2.7f;
        leadChance = 0.3f;
        speed = 1.2f;
        turnSpeed = 110;
        dodgeChance = 0.8f;
        
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
            StartCoroutine(stopMove(1.3f));
        }
    }
}

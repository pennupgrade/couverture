using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : ShieldedEnemy
{
    [HideInInspector] public Vector3 homePoint;
    void Awake() {
        enemyState = new Lance_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 300;
        gunRange = 9;
        sightRange = 11;
        FOV = 1f;
        rotSpeed = 72;
        reload = 4;
        bulletSpeed = 5.2f;
        leadChance = 0.5f;
        speed = 1.2f;
        turnSpeed = 180;
        dodgeChance = 0.8f;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        if (shieldEnabled) {
            shieldSetup();
            StartCoroutine(activateShield());
        }
        rb = GetComponent<Rigidbody>();
        homePoint = rb.position;
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
            StartCoroutine(stopMove(1.5f));
        }
    }
}

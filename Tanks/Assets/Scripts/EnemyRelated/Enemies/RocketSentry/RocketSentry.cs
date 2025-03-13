using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSentry : ShieldedEnemy
{
    void Awake() {
        enemyState = new RocketSentry_Idle(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 400;
        gunRange = 9;
        sightRange = 10;
        FOV = 1.2f;
        rotSpeed = 108;
        reload = 4f;
        bulletSpeed = 5.2f;
        leadChance = 0.2f;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject);
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

        gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
    }
}

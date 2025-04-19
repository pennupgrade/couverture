using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sentry : Enemy
{
    void Awake() {
        enemyState = new Sentry_Idle(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 300;
        gunRange = 8;
        sightRange = 8;
        FOV = 0.6f;
        rotSpeed = 60;
        reload = 4;
        bulletSpeed = 3;
        leadChance = 0.2f;
        
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

        gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
    }

    
}

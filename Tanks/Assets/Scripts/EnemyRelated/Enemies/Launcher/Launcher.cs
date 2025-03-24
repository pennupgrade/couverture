using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Enemy
{
    public float reloadValue;

    void Awake() {
        enemyState = new Launcher_Idle(this);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 100000;
        gunRange = 8;
        sightRange = 10;
        FOV = 2f;
        rotSpeed = 0;
        reload = reloadValue;
        bulletSpeed = 3;
        leadChance = 0;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 playerPos = Vector3.zero;
        enemyState = enemyState.Patrol(playerPos);
        enemyState = enemyState.Shoot(playerPos);
    }

    public override void takeDamage(int dmg) {
        
    }
}

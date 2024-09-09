using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian3 : EnemyOmniMove
{
    public GameObject minePrefab;
    void Awake() {
        enemyState = new G3_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 400;
        gunRange = 9;
        sightRange = 10;
        FOV = 0.9f;
        rotSpeed = 108;
        cooldownTime = 0.6f;
        reload = 3;
        magSize = 5;
        numBullets = magSize;
        bulletSpeed = 2.7f;
        leadChance = 0.33f;
        speed = 1.6f;
        turnSpeed = 180;
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

        //dodging
        stopTurns = detectBullet(2);
        if (stopTurns) {
            dodge();
        }

        //turning
        if (!stopTurns && moveStraightTimer == null) {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= cTurnSpeed * Time.fixedDeltaTime * Vector3.up; 
        }

        //moving
        if (accel && moveStraightTimer == null) {
            cSpeed = (backwards ? (Mathf.Max(-speed, cSpeed - 12 * Time.fixedDeltaTime)) : 
                                (Mathf.Min(speed, cSpeed + 12 * Time.fixedDeltaTime)));
        }
        transform.position += cSpeed * Time.fixedDeltaTime * transform.right;
    }
    public IEnumerator deployMines() {
        yield return new WaitForSeconds(4);
        while (true) {
            Object.Instantiate(minePrefab, transform.position - 0.15f * Vector3.up, Quaternion.identity);
            yield return new WaitForSeconds(16);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storm : EnemyOmniMove
{
    void Awake() {
        enemyState = new Storm_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 300;
        gunRange = 13;
        sightRange = 13;
        FOV = 1.5f;
        rotSpeed = 80;
        cooldownTime = 0.5f;
        reload = 5f;
        magSize = 2;
        numBullets = magSize;
        bulletSpeed = 4f;
        speed = 1.5f;
        turnSpeed = 160;
        
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
        stopTurns = detectBullet(1.8f);
        if (stopTurns) {
            dodge();
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= cTurnSpeed * Time.fixedDeltaTime * Vector3.up; 
            if (!accel) {
                alert();
            }
        }
        //turning
        else if (moveStraightTimer == null) {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= cTurnSpeed * Time.fixedDeltaTime * Vector3.up; 
        }

        //moving
        if (accel && moveStraightTimer == null) {
            cSpeed = (backwards ? (Mathf.Max(-speed, cSpeed - 10 * Time.fixedDeltaTime)) : 
                                (Mathf.Min(speed, cSpeed + 10 * Time.fixedDeltaTime)));
        }
        transform.position += cSpeed * Time.fixedDeltaTime * transform.forward;
    }

    public void fireRocket(bool left) {
        fireSound();
        Vector3 rocketDir = Quaternion.AngleAxis(36 * ((left) ? -1 : 1), Vector3.up) * gun.transform.forward;
        GameObject rocket = Instantiate(bulletPrefab, gunShotPos.position, Quaternion.LookRotation(rocketDir));
        bulletPrefab.GetComponent<HomingRocket2>().player = player;
    }
}

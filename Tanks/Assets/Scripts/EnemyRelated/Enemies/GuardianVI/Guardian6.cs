using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian6 : EnemyOmniMove
{
    void Awake() {
        enemyState = new G6_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 400;
        gunRange = 10;
        sightRange = 10;
        FOV = 1.8f;
        rotSpeed = 130;
        cooldownTime = 1.4f;
        reload = 5.5f;
        magSize = 2;
        numBullets = magSize;
        bulletSpeed = 6.2f;
        leadChance = 0.5f;
        speed = 2f;
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

    public void fire() {
        fireSound();
        GameObject bullet = Instantiate(bulletPrefab, gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(10 * (Random.value - 0.5f), Vector3.up)
         * (gun.transform.forward * bullet.GetComponent<Projectile>().bulletSpeed);

        bullet.GetComponent<Projectile>().parent = this.gameObject;
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
        bullet.GetComponent<RicochetRocket>().reduceBounces();
    }

    protected override IEnumerator reactivateShield() {
        yield return new WaitForSeconds(15);
        StartCoroutine(activateShield());
    }
}

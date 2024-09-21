using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian5 : EnemyOmniMove
{
    public GameObject minePrefab;
    void Awake() {
        enemyState = new G5_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 400;
        gunRange = 10;
        sightRange = 10;
        FOV = 1.5f;
        rotSpeed = 108;
        cooldownTime = 0.9f;
        reload = 4;
        magSize = 3;
        numBullets = magSize;
        bulletSpeed = 5.8f;
        leadChance = 0.5f;
        speed = 1.7f;
        turnSpeed = 180;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        shieldSetup();
        StartCoroutine(activateShield());
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
    public IEnumerator deployMines() {
        yield return new WaitForSeconds(4);
        while (true) {
            Object.Instantiate(minePrefab, transform.position - 0.15f * Vector3.up, Quaternion.identity);
            yield return new WaitForSeconds(16);
        }
    }
    protected override IEnumerator reactivateShield() {
        yield return new WaitForSeconds(15);
        StartCoroutine(activateShield());
    }
}

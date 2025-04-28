using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : EnemyOmniMove
{
    [HideInInspector] public bool invisActive, flickering;
    void Awake() {
        enemyState = new Shadow_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 300;
        gunRange = 7;
        sightRange = 8;
        FOV = 1.8f;
        rotSpeed = 108;
        cooldownTime = 0.33f;
        reload = 7f;
        magSize = 5;
        bulletSpeed = 5.2f;
        leadChance = 0.5f;
        speed = 1.5f;
        turnSpeed = 160;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        rb = GetComponent<Rigidbody>();
        damageFlash.CallDissolve(this, 2);
        invisActive = true;
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

    public void startInvisFlicker() => StartCoroutine(flickerCor());

    private IEnumerator flickerCor() {
        while (true) {
            yield return new WaitForSeconds(1.5f + 1.5f * Random.value);
            if (invisActive) {
                flickering = true;
                damageFlash.CallInvisFlicker(this, 1.5f);
                playSound("Ping");
                yield return new WaitForSeconds(1);
                flickering = false;
            }
        }
    }
    public override void takeDamage(int dmg) {
        health -= dmg;
        if (health <= 0 && !isDead) {
            die();
        } else if (invisActive) {
            damageFlash.CallInvisDamage(this, 0.3f);
        }
    }
    public void turnInvis(bool on) {
        if (on) {
            damageFlash.CallDissolve(this, 1.5f);
            StartCoroutine(setInvisActive(1.5f));
        } else {
            invisActive = false;
            damageFlash.CallDissolve(this, 1.5f, true);
        }
    }
    private IEnumerator setInvisActive(float delay) {
        yield return new WaitForSeconds(delay);
        invisActive = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Javelin : EnemyOmniMove
{
    public GameObject flashParticles;
    public LayerMask lm;
    [HideInInspector]
    public bool stationary;
    void Awake() {
        enemyState = new Javelin_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 200;
        gunRange = 6.5f;
        sightRange = 9;
        FOV = 1.25f;
        rotSpeed = 90;
        cooldownTime = 7;
        numBullets = 2;
        leadChance = 0.7f;
        speed = 1.8f;
        turnSpeed = 180;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;
        enemyState = enemyState.Patrol(playerPos);
        enemyState = enemyState.RotateTurret(playerPos);
        enemyState = enemyState.Shoot(playerPos);
        if (!stationary) {
            gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
        }
    }
    void FixedUpdate() {
        agent.nextPosition = transform.position;
        if (isStunned) return;

        //dodging
        stopTurns = detectBullet(1.7f);
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
        if (!stationary) {
            transform.position += cSpeed * Time.fixedDeltaTime * transform.forward;
        }
    }


    public IEnumerator muzzleFlash() {
        flashParticles.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(1.3f);
        flashParticles.GetComponent<ParticleSystem>().Stop();
    }
    public void fireBeam(float dist) {
        fireSound();
        RailgunLineScript ls = Instantiate(bulletPrefab).GetComponent<RailgunLineScript>();
        ls.dist = dist;
        ls.startPos = gunShotPos.position;
        ls.dir = gun.transform.forward;
        GameObject bulletExp = Instantiate(bulletExplosionPrefab, gunShotPos.position + dist * gun.transform.forward, Quaternion.identity);
        Destroy(bulletExp, 2);
    }

}

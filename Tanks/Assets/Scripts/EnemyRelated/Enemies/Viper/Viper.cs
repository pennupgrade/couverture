using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viper : ShieldedEnemy
{
    [SerializeField] GameObject laser;
    void Awake() {
        enemyState = new Viper_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 400;
        gunRange = 12;
        sightRange = 13;
        FOV = 1f;
        rotSpeed = 120;
        cooldownTime = 0.25f;
        magSize = 12;
        numBullets = magSize;
        speed = 1.2f;
        turnSpeed = 200;
        dodgeChance = 0.7f;
        
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
            StartCoroutine(stopMove(1.3f));
        }
    }
    public void fireRocket() {
        fireSound();
        GameObject rocket = Instantiate(bulletPrefab, gunShotPos.position, Quaternion.LookRotation(gun.transform.forward));
        bulletPrefab.GetComponent<HomingRocket>().player = player;
    }

    public void toggleLaser() {
        laser.SetActive(!laser.activeSelf);
    }
}

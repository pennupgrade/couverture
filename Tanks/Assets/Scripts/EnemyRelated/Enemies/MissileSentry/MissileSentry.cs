using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSentry : ShieldedEnemy
{
    public GameObject laser;
    void Awake() {
        enemyState = new MissileSentry_Idle(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 500;
        gunRange = 12;
        sightRange = 13;
        FOV = 1.4f;
        rotSpeed = 70;
        cooldownTime = 0.25f;
        magSize = 14;
        numBullets = magSize;
        
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

    public void fireRocket() {
        fireSound();
        GameObject rocket = Instantiate(bulletPrefab, gunShotPos.position, Quaternion.LookRotation(gun.transform.forward));
        bulletPrefab.GetComponent<HomingRocket>().player = this.player;

    }

    public void toggleLaser() {
        laser.SetActive(!laser.activeSelf);
    }
}

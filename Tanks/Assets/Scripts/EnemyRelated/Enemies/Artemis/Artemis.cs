using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Artemis : Enemy
{
    public GameObject laser;
    [HideInInspector] public bool stopTurnA;
    [HideInInspector] public Vector3 homePoint;
    void Awake() {
        enemyState = new Artemis_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 400;
        gunRange = 14;
        sightRange = 14;
        FOV = 1.6f;
        rotSpeed = 13;
        reload = 3f;
        speed = 1;
        turnSpeed = 120;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        rb = GetComponent<Rigidbody>();
        homePoint = rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        enemyState = enemyState.Patrol(playerPos);
        enemyState = enemyState.RotateTurret(playerPos);
        enemyState = enemyState.Shoot(playerPos);

        if (!stopTurnA) {
            gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
        }
    }
    void FixedUpdate() {
        if (playerRB == null) return;

        agent.nextPosition = transform.position;
        if (isStunned) return;
        //turning
        if (!stopTurns && moveStraightTimer == null) {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= cTurnSpeed * Time.fixedDeltaTime * Vector3.up; 
        }
        //moving
        transform.position += cSpeed * Time.fixedDeltaTime * transform.forward;
    }
    void OnCollisionEnter(Collision collision) {
        if ((collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Tank")
             && cSpeed > 0.01f){
            StartCoroutine(stopMove(1.5f));
        }
    }

    public bool hitPlayer() {
        //Debug.DrawRay(gunShotPos.position, gunShotPos.forward * 15, Color.red, 1); 
        if (Physics.Raycast(gunShotPos.position, gunShotPos.forward, out RaycastHit hit, 15, 1 << 2))
        { 
            //Debug.Log("raycast hit");
            fireSound();
            pTank.takeDamage(100);
            GameObject explosion = Instantiate(bulletPrefab, hit.point, Quaternion.identity);
            Destroy(explosion, 3);
            return true;
        }
        return false;
    }

    public void toggleLaser() {
        laser.SetActive(!laser.activeSelf);
    }
}

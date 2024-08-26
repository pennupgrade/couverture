using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scout : Enemy
{
    public bool warningSent;
    public GameObject signalPrefab;
    void Awake() {
        enemyState = new Scout_Start(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 200;
        gunRange = 8;
        sightRange = 9;
        FOV = 1.2f;
        rotSpeed = 120;
        reload = 3;
        bulletSpeed = 2;
        leadChance = 0.25f;
        speed = 1.5f;
        turnSpeed = 100;
        
        damageFlash = new DamageFlash(transform.Find("Body").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
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
    void FixedUpdate() {
        if (playerRB == null) return;

        agent.nextPosition = transform.position;
        if (isStunned) return;
        //turning
        if (!stopTurns && moveStraightTimer == null) {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= 0.5f * cTurnSpeed * Time.fixedDeltaTime * Vector3.up; 
        }
        //moving
        transform.position += cSpeed * Time.fixedDeltaTime * transform.right;
    }
    void OnCollisionEnter(Collision collision) {
        if ((collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Tank")
             && cSpeed > 0.01f){
            StartCoroutine(stopMove(1.2f));
        }
    }

    public void signalFlare() {
        GameObject flare = Instantiate(signalPrefab, transform.position + 0.2f * Vector3.up, Quaternion.identity);
        Destroy(flare, 10);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StandardTank : EnemyPathing
{
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agentSetup();
    }
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        activeRadius = 7;
        state = EnemyState.Start;
        rotSpeed = 108;
        dispersion = 24;
        reload = 3;
        bulletSpeed = 2.5f;
        speed = 1.2f;
        turnSpeed = 80;
        leadChance = 0.2f;
        //coroutine for idle turret turning
        StartCoroutine(idleTurn());
        //coroutine for recalculating path to destination
        StartCoroutine(calcPath1());
    }

    // Update is called once per frame
    void Update()
    {
        // changes between states depending on whether player is detected
        if (checkTimer < 0.01f) {
            if (checkIfPlayerDetected()) {
                state = EnemyState.Alert;
                //can alert another enemy to start patrolling
                if (enemyToAlert != null) {
                    enemyToAlert.alertEnemy();
                }
                checkTimer = 7;
            } else if (state != EnemyState.Start) {
                state = EnemyState.Idle;
                checkTimer = 1 + Random.value;
            }
        }

        // conditions for shooting
        if (state == EnemyState.Alert && reloadTimer < 0.01f) {
            hasLineOfSight = lineOfSightCheck();
            if (hasLineOfSight && isAimed()) {
                fire();
                reloadTimer = reload;
            } else {
                reloadTimer = 0.16f;
            }
        }

        // when to set a new destination
        if (waypointTimer < 0.01f || hasReachedDest()) {
            waypointTimer = 4 + 4 * Random.value;
            destination = getRandomPoint(8);
            agent.SetDestination(destination);
        }

        reloadTimer = TimerF(reloadTimer);
        checkTimer = TimerF(checkTimer);
        waypointTimer = TimerF(waypointTimer);
    }
    void FixedUpdate() {
        //turret turning
        if (state == EnemyState.Alert) {
            turnTurret();
        }
        gun.transform.eulerAngles += cTurn * Time.fixedDeltaTime * Vector3.up; 

        //tank doesn't move at start, until player is first detected or alerted by another enemy
        if (state == EnemyState.Start) {
            return;
        }
        //necessary for controlling the NavAgent yourself
        agent.nextPosition = transform.position;
        //move forwards, unless a wall was hit
        if (!stopMovement) {
            rb.velocity = speed * transform.right;
        }
        //tank turning
        turnTowardsPath();
    }
    void OnCollisionEnter(Collision collision) {
        if ((collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Tank")
             && !stopMovement){
            StartCoroutine(stopMove());
        }
    }
}

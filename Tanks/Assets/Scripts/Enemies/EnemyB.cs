using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyB : EnemyPathing
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
        state = EnemyState.Idle;
        rotSpeed = 90;
        dispersion = 24;
        reload = 3;
        bulletSpeed = 3;
        speed = 1.2f;
        turnSpeed = 80;
        StartCoroutine(idleTurn());
        StartCoroutine(calcPath1());
    }

    // Update is called once per frame
    void Update()
    {
        if (checkTimer < 0.01f) {
            if (playerCheck()) {
                state = EnemyState.Alert;
                checkTimer = 7;
            } else {
                state = EnemyState.Idle;
                checkTimer = 1.5f + Random.value;
            }
        }

        if (state == EnemyState.Alert && reloadTimer < 0.01f) {
            hasLineOfSight = lineOfSightCheck();
            aimReady = isAimed();
            if (hasLineOfSight && aimReady) {
                fire();
                reloadTimer = reload;
            } else {
                reloadTimer = 0.25f;
            }
        }

        if (waypointTimer < 0.01f || hasReachedDest()) {
            waypointTimer = 6 + 6 * Random.value;
            destination = getRandomPoint(8);
            agent.SetDestination(destination);
        }

        reloadTimer = TimerF(reloadTimer);
        checkTimer = TimerF(checkTimer);
        waypointTimer = TimerF(waypointTimer);
    }
    void FixedUpdate() {
        if (state == EnemyState.Alert) {
            turnTurret();
        }
        gun.transform.eulerAngles += cTurn * Time.fixedDeltaTime * Vector3.up; 

        if (state == EnemyState.Idle) {
            return;
        }
        agent.nextPosition = transform.position;
        if (!stopMovement) {
            rb.velocity = speed * transform.right;
        }
        turnTowardsPath();
    }
    void OnCollisionEnter(Collision collision) {
        if ((collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Tank")
             && !stopMovement){
            StartCoroutine(stopMove());
        }
    }
}

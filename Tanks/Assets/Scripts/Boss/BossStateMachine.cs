using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    // Basic State Information
    private State currentState;
    private Attack currentAttack;

    // Other Parameters
    private double timer;
    private GameObject player;
    [SerializeField] private Boss boss;

    // Constants
    private double timeUntilAttack;
    private const double MELEE_DISTANCE = 4f;
    private const double TIME_BETWEEN_BULLETS = 0.5f;
    private const int MAX_NUMBER_SUMMONS = 2;
    private float chargeStartTime = 0f;
    // Attack Specific Parameters
    private int numBulletsShot;
    private List<List<GameObject>> enemies;
    

    [SerializeField] private float chargeCD;
    [SerializeField] private float chargeRange;

    [SerializeField] private GameObject indicatorObject;
    [SerializeField] private BossMovement bm;
    private Vector3 chargeStart;


    private void Awake()
    {
        currentState = State.Idle;
        currentAttack = Attack.None;
        timer = 0;
        enemies = new List<List<GameObject>>();
        

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Could not find player");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (currentState == State.Idle)
        {
            if (timer > timeUntilAttack)
            {
                switchToState(State.Attacking);
            }
            doIdleState();
        }
        else if (currentState == State.Attacking)
        {
            doAttackState();
        }

        // Remove summon enemies if killed
        for (int i = 0; i < enemies.Count; i++)
        {
            for (int j = 0; j < enemies[i].Count; j++)
            {
                if (enemies[i][j] == null)
                {
                    enemies[i].RemoveAt(j);
                    j--;
                }
            }
            if (enemies[i].Count == 0)
            {
                enemies.RemoveAt(i);
                    i--;
            }
        }
    }

    private void switchToState(State newState)
    {
        currentState = newState;
        if (newState == State.Idle)
        {
            Debug.Log("Switching To Idle State");
            timer = 0;
            currentAttack = Attack.None;
            return;
        }
        if (newState == State.Attacking)
        {
            if (!boss.isUnplugged()) {
                timeUntilAttack = UnityEngine.Random.Range(4, 6);
            } else {
                timeUntilAttack = UnityEngine.Random.Range(3, 4);
            }
            
            Debug.Log("Switching To Attack State");
            if (Vector3.Distance(player.transform.position, transform.position) < MELEE_DISTANCE)
            {
                // Chose between shotgun and melee
                int rand = UnityEngine.Random.Range(0, 2);
                if (rand == 0 && Time.time > chargeStartTime + chargeCD) {
                    currentAttack = Attack.Charge;
                    if (Time.time > chargeStartTime + chargeCD) {
                        chargeStart = this.transform.position;
                        chargeStartTime = Time.time;

                        Vector3 chargeDir = player.transform.position - transform.position;
                        chargeDir.Normalize();
                        chargeDir.y = 0;
                        Vector3 indicatorLoc = this.transform.position + chargeDir * chargeRange/2;
                        Vector3 rot = Quaternion.LookRotation(chargeDir).eulerAngles;
                        rot.x = -90;
                        Instantiate(indicatorObject, indicatorLoc, Quaternion.Euler(rot));
                    }
                } else {
                    currentAttack = Attack.Shotgun;
                }
                
            }
            else
            {
                // Choose between summon, shoot, and charge
                int rand = UnityEngine.Random.Range(0, 4);
                if (enemies.Count < MAX_NUMBER_SUMMONS && rand == 0) 
                {
                    currentAttack = Attack.Summon;
                } else if (Time.time > chargeStartTime + chargeCD && rand == 1)
                {
                    currentAttack = Attack.Charge;
                    if (Time.time > chargeStartTime + chargeCD) {
                        chargeStart = this.transform.position;
                        chargeStartTime = Time.time;

                        Vector3 chargeDir = player.transform.position - transform.position;
                        chargeDir.Normalize();
                        chargeDir.y = 0;
                        Vector3 indicatorLoc = this.transform.position + chargeDir * chargeRange/2;
                        Vector3 rot = Quaternion.LookRotation(chargeDir).eulerAngles;
                        rot.x = -90;
                        Instantiate(indicatorObject, indicatorLoc, Quaternion.Euler(rot));
                    }
                
                } else {
                    currentAttack = Attack.Shoot;
                    timer = TIME_BETWEEN_BULLETS;
                    numBulletsShot = 0;
                }
            }
        }
    }

    private void doIdleState()
    {
        // Track Onto Player
    }

    private void doAttackState()
    {
        switch (currentAttack)
        {
            // Shoots 3 shots in rapid succession. When done, switches back to idle.
            case Attack.Shoot:
                if (timer > TIME_BETWEEN_BULLETS)
                {
                    timer = 0;
                    boss.shootBullet();
                    numBulletsShot++;
                    if (numBulletsShot >= 3)
                    {
                        switchToState(State.Idle);
                    }
                }
                break;
            // Shots a spread of three bullets and then switches to idle
            case Attack.Shotgun:
                boss.shootShotgun();
                switchToState(State.Idle);
                break;
            // Summons a wave of enemies and then switches to idle
            case Attack.Summon:
                enemies.Add(boss.Summon());
                switchToState(State.Idle);
                break;
            case Attack.Charge:
                boss.Charge(chargeStartTime); 
                if (Time.time > chargeStartTime + 2f) {
                    switchToState(State.Idle);
                }
                break;
        }
    }


    private enum State
    {
        Idle,
        Attacking
    }

    private enum Attack
    {
        Shotgun,
        Shoot,
        Charge,
        Summon,
        Melee,
        None
    }

    void OnCollisionEnter(Collision col) {
        Debug.Log("collided");
        if (currentAttack == Attack.Charge && col.gameObject.tag == "Player") {
            Tank tank = col.gameObject.GetComponent<Tank>();
            tank.takeDamage(50);
            Vector3 diff = player.transform.position - this.transform.position;
            Vector3 knock = new Vector3(diff.x, 2f, diff.z) * 30000;
            col.gameObject.GetComponent<Rigidbody>().AddForce(knock, ForceMode.Impulse);
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossStateMachine : MonoBehaviour
{
    // Basic State Information
    public State currentState;
    public Attack currentAttack;

    // Other Parameters
    private double timer;
    private GameObject player;
    [SerializeField] private Boss boss;

    // Constants
    private double timeUntilAttack;
    private const double MELEE_DISTANCE = 4f;
    private const double TIME_BETWEEN_BULLETS = 0.5f;
    private const int MAX_NUMBER_SUMMONS = 2;
    private float chargeStartTime;

    // Attack Specific Parameters
    private int numBulletsShot;
    private List<List<GameObject>> enemies;
    private bool chargeHitAlready;
    private float missileTimer;
    private float missileCheckTime;
    private const float missileShootProbability = 0.2f;

    public Tank playerTank;

    public float stunDur = 1f;
    private float stunStart;
    public bool playerIsStunned;

    [SerializeField] private GameObject wall;
    

    [SerializeField] private float chargeCD;
    [SerializeField] private float chargeRange;

    [SerializeField] private GameObject indicatorObject;
    [SerializeField] private BossMovement bm;
    private Vector3 chargeStart;
    private Vector3 chargeDest;

    private float startTime;
    private Vector3 lastPos;
    [SerializeField] private Animator anim;
    

    private void Awake()
    {
        currentState = State.Idle;
        currentAttack = Attack.None;
        timer = 0;
        missileTimer = 0;
        missileCheckTime = 3f;
        enemies = new List<List<GameObject>>();
        

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Could not find player");
        }
        chargeStartTime = 0f;
        playerIsStunned = false;
    }

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > stunDur + stunStart && playerIsStunned) {
            player.transform.position = lastPos;
            playerTank.enabled = true;
            player.GetComponent<Rigidbody>().isKinematic = true;
            playerTank.ResetPosition(lastPos);
            playerIsStunned = false;
        } else {
            lastPos = player.transform.position;
        }

        print(wall.transform.position.y);
        if (wall.transform.position.y > -2)
        {
            print("WHAT");   
            timer += Time.deltaTime;
            missileTimer += Time.deltaTime;
        }
        
        if (currentState == State.Idle)
        {
            if (missileTimer > missileCheckTime)
            {
                float yScaleFactor = ((player.transform.position.y + 1.244195f) / (1.244f-0.4f)) * 2f + 1f;

                if (UnityEngine.Random.Range(0f, 1f) <= missileShootProbability * yScaleFactor)
                {
                    boss.ShootMissile();
                }
                missileTimer = 0f;
            }
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
        chargeHitAlready = false;
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
                missileCheckTime = 2f;
            }
            
            Debug.Log("Switching To Attack State");
            if (Vector3.Distance(player.transform.position, transform.position) < MELEE_DISTANCE && Time.time > startTime + 10f)
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
                        boss.chargeDir = chargeDir;
                        Vector3 rot = Quaternion.LookRotation(chargeDir).eulerAngles;
                        rot.x = -90;
                        GameObject ind = Instantiate(indicatorObject, indicatorLoc, Quaternion.Euler(rot));
                        ind.transform.localScale = new Vector3(1,chargeRange + 0.5f, 1);
                        chargeStart = this.transform.position;
                        anim.SetTrigger("Charge");
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
                } else if (Time.time > chargeStartTime + chargeCD && rand == 1 && Time.time > startTime + 10f)
                {
                    currentAttack = Attack.Charge;
                    if (Time.time > chargeStartTime + chargeCD) {
                        chargeStart = this.transform.position;
                        chargeStartTime = Time.time;

                        Vector3 chargeDir = player.transform.position - transform.position;
                        chargeDir.Normalize();
                        chargeDir.y = 0;
                        Vector3 indicatorLoc = this.transform.position + chargeDir * chargeRange/2;
                        boss.chargeDir = chargeDir;
                        Vector3 rot = Quaternion.LookRotation(chargeDir).eulerAngles;
                        rot.x = -90;
                        GameObject ind = Instantiate(indicatorObject, indicatorLoc, Quaternion.Euler(rot));
                        ind.transform.localScale = new Vector3(1,chargeRange + 0.5f, 1);
                        chargeStart = this.transform.position;
                        anim.SetTrigger("Charge");
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
                    if (numBulletsShot >= 5)
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
                
                //boss.Charge(chargeStartTime, chargeStart, chargeRange); 
                if (Time.time > chargeStartTime + 2f ||
                    (this.transform.position - chargeStart).magnitude > chargeRange) {
                    switchToState(State.Idle);
                }
                break;
        }
    }


    public enum State
    {
        Idle,
        Attacking
    }

    public enum Attack
    {
        Shotgun,
        Shoot,
        Charge,
        Summon,
        Melee,
        None
    }

    void OnCollisionEnter(Collision col) {
        Debug.Log("collided " + col.gameObject.tag + currentAttack + chargeHitAlready);
        if (currentAttack == Attack.Charge && col.gameObject.tag == "Player" && !chargeHitAlready && !playerIsStunned) {
            col.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            Debug.Log("Player hit!");
            Tank tank = col.gameObject.GetComponent<Tank>();
            Vector3 diff = (player.transform.position - this.transform.position).normalized;
            Vector3 add = new Vector3 (UnityEngine.Random.Range(0.5f, 1.5f), 1, UnityEngine.Random.Range(0.5f, 1.5f));
            Vector3 knock = new Vector3(diff.x * add.x, 2.5f, diff.z * add.z) * 8000;

            playerTank.enabled = false;
            playerIsStunned = true;
            stunStart = Time.time;

            col.gameObject.GetComponent<Rigidbody>().AddForce(knock, ForceMode.Impulse);
            switchToState(State.Idle);
            tank.takeDamage(50);
            chargeHitAlready = true;
        }
    }

}

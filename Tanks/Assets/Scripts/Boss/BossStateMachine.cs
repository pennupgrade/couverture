using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

    public float startTime;
    private Vector3 lastPos;
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject playerDummy;
    [SerializeField] private GameObject damageZone;
    private PlayerCamera pc;
    private Transform dummyTransform;
    private bool isAggro;

    private void Awake() {
        currentState = State.Idle;
        currentAttack = Attack.None;
        timer = 0;
        missileTimer = 0;
        missileCheckTime = 3f;
        enemies = new List<List<GameObject>>();

        pc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerCamera>();

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("Could not find player");
        }

        chargeStartTime = 0f;
        playerIsStunned = false;
        boss.setEnemies(enemies);
    }

    private void Update() {
        chargeRange = 3.5f;

        if (playerIsStunned) {
            if (dummyTransform.GetComponent<TankDummy>().StunOver()) {
                player.transform.position = dummyTransform.position;
                Destroy(dummyTransform.gameObject);
                playerTank.ResetPosition(dummyTransform.position);
                pc.SetPlayer(player);
                bm.SetPlayer(player);
                playerTank.takeDamage(25);
                //playerTank.gameObject.SetActive(true);

                //reenables Player
                playerTank.GetComponent<Collider>().enabled = true;
                playerTank.enabled = true;
                playerTank.transform.GetChild(1).gameObject.SetActive(true);

                //playerTank.tankState.ResetReload();
                playerIsStunned = false;
            }
        }
        else {
            lastPos = player.transform.position;
        }

        if (boss.isStarted || wall.transform.position.y > -2) {
            boss.isStarted = true;
            if (!isAggro) {
                isAggro = true;
                startTime = Time.time;
            }

            timer += Time.deltaTime;
            missileTimer += Time.deltaTime;
        }

        if (currentState == State.Idle) {
            if (missileTimer > missileCheckTime) {
                var yScaleFactor = (player.transform.position.y + 1.244195f) / (1.244f - 0.4f) * 2f + 1f;
                if (UnityEngine.Random.Range(0f, 1f) <= missileShootProbability * yScaleFactor) {
                    boss.ShootMissile();
                }

                missileTimer = 0f;
            }

            if (timer > timeUntilAttack) {
                switchToState(State.Attacking);
            }

            doIdleState();
        }
        else if (currentState == State.Attacking) {
            doAttackState();
        }

        // Remove summon enemies if killed
        for (var i = 0; i < enemies.Count; i++) {
            for (var j = 0; j < enemies[i].Count; j++) {
                if (enemies[i][j] == null) {
                    enemies[i].RemoveAt(j);
                    j--;
                }
            }

            if (enemies[i].Count == 0) {
                enemies.RemoveAt(i);
                i--;
            }
        }
    }

    private void switchToState(State newState) {
        chargeHitAlready = false;
        currentState = newState;
        if (newState == State.Idle) {
            Debug.Log("Switching To Idle State");
            timer = 0;
            currentAttack = Attack.None;
            return;
        }

        if (newState == State.Attacking) {
            if (!boss.isUnplugged()) {
                timeUntilAttack = UnityEngine.Random.Range(4, 6);
            }
            else {
                timeUntilAttack = UnityEngine.Random.Range(2.5f, 3f);
                missileCheckTime = 2f;
            }

            Debug.Log("Switching To Attack State");
            if (Vector3.Distance(player.transform.position, transform.position) < MELEE_DISTANCE) {
                // Chose between shotgun and melee
                var rand = UnityEngine.Random.Range(0, 2);
                if (rand == 0 && Time.time > chargeStartTime + chargeCD) {
                    currentAttack = Attack.Charge;
                    if (Time.time > chargeStartTime + chargeCD && Time.time > startTime + 5f) {
                        InitializeCharge();
                    }
                }
                else {
                    currentAttack = Attack.Shotgun;
                }
            }
            else {
                // Choose between summon, shoot, and charge
                var rand = UnityEngine.Random.Range(0, 5);
                if (enemies.Count < MAX_NUMBER_SUMMONS && rand <= 1) {
                    currentAttack = Attack.Summon;
                }
                else if (Time.time > chargeStartTime + chargeCD && rand == 2) {
                    currentAttack = Attack.Charge;
                    if (Time.time > chargeStartTime + chargeCD && Time.time > startTime + 5f) {
                        InitializeCharge();
                    }
                }
                else {
                    currentAttack = Attack.Shoot;
                    timer = TIME_BETWEEN_BULLETS;
                    numBulletsShot = 0;
                }
            }
        }
    }

    private void doIdleState() {
        // Track Onto Player
    }

    private void doAttackState() {
        switch (currentAttack) {
        // Shoots 3 shots in rapid succession. When done, switches back to idle.
        case Attack.Shoot:
            if (timer > TIME_BETWEEN_BULLETS) {
                timer = 0;
                boss.shootBullet();
                numBulletsShot++;
                if (numBulletsShot >= 5) {
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
            if (Time.time > chargeStartTime + 1f) {
                GetComponent<Rigidbody>().isKinematic = false;
            }
            if (Time.time > chargeStartTime + 2.2f ||
                (transform.position - chargeStart).magnitude > chargeRange) {
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

    private void OnCollisionEnter(Collision col) {
        Debug.Log("collided " + col.gameObject.tag + currentAttack + chargeHitAlready);
        if (currentAttack == Attack.Charge && col.gameObject.tag == "Player" && !chargeHitAlready && !playerIsStunned) {
            var dummy = Instantiate(playerDummy, col.transform.position, col.transform.rotation);
            dummyTransform = dummy.transform;

            var tank = col.gameObject.GetComponent<Tank>();
            var diff = (player.transform.position - transform.position).normalized;
            var add = new Vector3(UnityEngine.Random.Range(0.5f, 1.5f), 1, UnityEngine.Random.Range(0.5f, 1.5f));
            var knock = new Vector3(diff.x * add.x, 1f, diff.z * add.z) * 5;

            //new Vector3(* 0.003f, 20f, diff.z * add.z * 0.003f);

            dummy.GetComponent<Rigidbody>().AddForce(knock, ForceMode.Impulse);

            //print("DIFF" + diff + " add" + add + " KNOCK" + knock + " " + player.transform.position + " " + this.transform.position);
            playerIsStunned = true;
            stunStart = Time.time;
            pc.SetPlayer(dummy);
            bm.SetPlayer(dummy);

            playerTank.GetComponent<Collider>().enabled = false;
            playerTank.enabled = false;
            playerTank.transform.GetChild(1).gameObject.SetActive(false);

            switchToState(State.Idle);
            chargeHitAlready = true;
        }

        if (currentAttack == Attack.Charge && col.gameObject.tag == "Environment") {
            //anim.SetTrigger("Stun");
        }
    }

    private void InitializeCharge() {
        GetComponent<Rigidbody>().isKinematic = true;
        chargeStart = transform.position;
        chargeStartTime = Time.time;
        var chargeDir = player.transform.position - transform.position;
        chargeDir.Normalize();
        chargeDir.y = 0;
        this.transform.rotation = Quaternion.LookRotation(chargeDir);
        var indicatorLoc = transform.position + chargeDir * chargeRange / 2 - chargeDir * 0.25f;
        var rot = Quaternion.LookRotation(chargeDir).eulerAngles;
        rot.x = -90;
        var ind = Instantiate(indicatorObject, indicatorLoc, Quaternion.Euler(rot));
        ind.transform.localScale = new Vector3(1, chargeRange + 0.5f, 1);
        chargeStart = transform.position;
        anim.SetTrigger("Charge");
    }

    private bool CanCharge() {
        Vector3 chargeDir = (player.transform.position - transform.position) * -1;
        chargeDir.Normalize();
        chargeDir.y = 0;
        return !Physics.Raycast(transform.position, chargeDir, -1.6867f, LayerMask.GetMask("Obstacle"));
    }

}
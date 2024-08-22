using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDestroyable, IAlertableEnemy
{
    protected Enemy_State enemyState;
    // these next two are set on individual basis
    [Tooltip("If true, tank moves in straight line upon activation.")]
    public bool straightLineAtStart;
    [Tooltip("Distance from player when tank is activated, if range = 0, tank is only activated when enemy goes into alert state.")]
    public float moveStartRange = 0;

    // Object References
    public GameObject gun;
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public Transform gunShotPos;
    public Rigidbody rb;
    public GameObject player;
    public Rigidbody playerRB;
    public Tank pTank;

    // Damage related
    public delegate void OnDeath();
    public event OnDeath onDeath;
    protected DamageFlash damageFlash;

    // Coroutines
    public Coroutine activeShootPeriodically;
    public Coroutine moveStraightTimer;
    public Coroutine idleTurretCor;
    public Coroutine alertPatrol;
    public Coroutine reloadCor;
    public Coroutine wayPointUpdate;

    // movement
    public float speed, turnSpeed, cSpeed;
    [SerializeField] protected bool stopTurns;
    public Vector3 destination;
    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;

    // the rest
    [SerializeField] protected int health;
    protected bool isDead, isStunned;
    [HideInInspector] public float FOV;
    [HideInInspector] public float sightRange, gunRange;
    [HideInInspector] public float reload;
    [HideInInspector] public float bulletSpeed;
    public float rotSpeed, cTurretTurn; // turrets
    [HideInInspector] public Vector3 TargetDir;
    [HideInInspector] public float leadChance;
    
    // bullet stuff
    public int numBullets;
    [HideInInspector] public float cooldownTime;


    protected void findPlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("Could not find player");
        } else {
            playerRB = player.GetComponent<Rigidbody>();
            pTank = player.GetComponent<Tank>();
        }
    }
    protected void agentSetup() {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    protected IEnumerator stopMove(float length) {
        cSpeed = 0;
        yield return new WaitForSeconds(length);
        cSpeed = speed;
    }
//-----------------------------------------------------
    protected float TimerF(float val)
    {
        if (val > 0)
        {
            val -= Time.deltaTime;
            if (val <= 0) val = 0;
        }
        return val;
    }
     public void takeDamage(int dmg) {
        health -= dmg;
        damageFlash.CallDamageFlash(this);
        if (health <= 0 && !isDead) {
            isDead = true;
            onDeath?.Invoke();
            destruction();
            onDeath = null;
        }
    }
    public void incapacitate(float time) {
        StartCoroutine(stunTimer(time));
    }
    private IEnumerator stunTimer(float time) {
        isStunned = true;
        yield return new WaitForSeconds(time);
        isStunned = false;
    }

    public virtual void alert() {
        if (enemyState is EnemyStartState) {
            ((EnemyStartState)enemyState).ChangeToIdle();
        }
    }

    protected virtual void destruction() {
        if (explosionPrefab != null) {
            GameObject expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(expl, 2);
        }
        Destroy(gameObject);
    }

}
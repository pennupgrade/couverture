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
    public GameObject bulletExplosionPrefab;
    public Transform gunShotPos;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public GameObject player;
    [HideInInspector] public Rigidbody playerRB;
    [HideInInspector] public Tank pTank;

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
    public Coroutine avoidBulletCor;

    // movement
    [HideInInspector] public float speed, turnSpeed, cSpeed, cTurnSpeed;
    [HideInInspector] public float dodgeChance;
    [HideInInspector] protected bool stopTurns;
    [HideInInspector] public Vector3 destination;
    [HideInInspector] public UnityEngine.AI.NavMeshAgent agent;

    // the rest
    [SerializeField] protected int health;
    protected bool isDead, isStunned;
    [HideInInspector] public float FOV;
    [HideInInspector] public float sightRange, gunRange;
    [HideInInspector] public float reload;
    [HideInInspector] public float rotSpeed, cTurretTurn; // turrets
    [HideInInspector] public Vector3 TargetDir;
    [HideInInspector] public float leadChance;
    
    // bullet stuff
    public int numBullets, magSize;
    [HideInInspector] public float bulletSpeed;
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
        yield return new WaitForSeconds(0.1f);
        if (moveStraightTimer != null) {
            cSpeed = speed;
            yield break;
        }
        yield return new WaitForSeconds(length);
        cSpeed = speed;
    }
    //----------------------------------bullet dodging--------------------------------------
    protected void turnTowardsVector(Vector3 v) {
        float dir = Vector3.Dot(-transform.right, v);
        if (dir > 0.02f) {
            if (cTurnSpeed > 0) {
                cTurnSpeed -= 720 * Time.deltaTime;
            } else {
                cTurnSpeed = Mathf.Max(-turnSpeed, cTurnSpeed - 720 * Time.deltaTime);
            }
        } else if (dir < -0.02f) {
            if (cTurnSpeed < 0) {
                cTurnSpeed += 720 * Time.deltaTime;
            } else {
                cTurnSpeed = Mathf.Min(turnSpeed, cTurnSpeed + 720 * Time.deltaTime);
            }
        } else {
            cTurnSpeed = 0;
        }
    }
    protected IEnumerator avoidBullet(Vector3 desired, Rigidbody bullet) {
        stopTurns = true;
        while (Vector3.Dot(desired, transform.forward) < 0.995f){
            turnTowardsVector(desired);
            transform.eulerAngles += cTurnSpeed * Time.deltaTime * Vector3.up;
            gun.transform.eulerAngles -= 0.5f * cTurnSpeed * Time.deltaTime * Vector3.up;
            yield return null;
        }
        while (bullet != null && Vector3.Distance(transform.position, bullet.position) > 
                Vector3.Distance(transform.position, bullet.position + bullet.velocity.normalized)) {
            yield return new WaitForSeconds(0.16f);
        }
        stopTurns = false;
        avoidBulletCor = null;
    }
    public virtual void bulletWarn(Rigidbody bullet) {
        if (bullet == null || isStunned || avoidBulletCor != null || dodgeChance < Random.value) {
            return;
        }
        if (cSpeed == 0) {
            alert();
            return;
        }
        Vector3 rbForward = bullet.velocity.normalized;
        Vector3 badDir;
        if (MyMath.InterceptDirection(rb.position, bullet.position, cSpeed * transform.forward, 
                bullet.velocity.magnitude, out Vector3 result)){
            badDir = result;
            //Debug.DrawRay(bullet.position, 2 * badDir, Color.red, 1);
        } else return;

        if (Vector2.Dot(new Vector2(rbForward.x, rbForward.z),
                new Vector2(badDir.x, badDir.z)) > 0.82f) {
            float frontDot = Vector3.Dot(transform.forward, rbForward);
            if (frontDot > 0.58f || frontDot < -0.75f) {
                avoidBulletCor = StartCoroutine(avoidBullet((Random.value > 0.5f) ? transform.right : -transform.right, bullet));
            } else {
                avoidBulletCor = StartCoroutine(avoidBullet((frontDot > 0) ? rbForward : -rbForward, bullet));
            }
        }
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
    public virtual void takeDamage(int dmg) {
        health -= dmg;
        damageFlash.CallDamageFlash(this);
        alert();
        if (health <= 0 && !isDead) {
            die();
        }
    }
    protected void die() {
        isDead = true;
        onDeath?.Invoke();

        destruction();
        onDeath = null;
    }
    public void incapacitate(float time) {
        StartCoroutine(stunTimer(time));
    }
    private IEnumerator stunTimer(float time) {
        damageFlash.CallElectricity(this, time);
        isStunned = true;
        yield return new WaitForSeconds(time);
        isStunned = false;
    }
    public int getHealth() {
        return health;
    }

    public virtual void alert() {
        if (enemyState is EnemyStartState) {
            ((EnemyStartState)enemyState).ChangeToIdle();
        }
    }

    public void spawnBulletBoom()
    {
        Vector3 pos = transform.position;

        float randX = Random.Range(-1, 1f);
        float randY = Random.Range(-1, 1f);
        float randZ = Random.Range(-1, 1f);

        Vector3 randPos = new Vector3(randX, randY, randZ);
        randPos.Normalize();

        float range = 0.15f;
        randPos *= range;
        randPos += pos;

        GameObject expl = Instantiate(bulletExplosionPrefab, randPos, Quaternion.identity);
        ParticleSystem.MainModule pMain = expl.GetComponent<ParticleSystem>().main;
        pMain.startSize = new ParticleSystem.MinMaxCurve(0.07f, 0.3f);

        Destroy(expl, 2);
    }

    protected virtual void destruction() {
        if (explosionPrefab != null) {
            GameObject expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(expl, 5);
        }
        if (bulletExplosionPrefab != null)
        {
            // 4am code
            for (int i = 0; i < 4; i++)
            {
                spawnBulletBoom();
            }
        }
        Destroy(gameObject);
    }

}
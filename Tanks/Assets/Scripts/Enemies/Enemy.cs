using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDestroyable
{
    // Object References
    public GameObject gun;
    public GameObject bulletPrefab;
    public Transform gunShotPos;
    public Rigidbody rb;
    public Rigidbody playerRB;
    public delegate void OnDeath();
    public event OnDeath onDeath;
    protected DamageFlash damageFlash;

    public bool hasLineOfSight;
    public EnemyState state;
    protected float activeRadius;
    protected float rotSpeed, cTurn; // turning for turrets
    protected float checkTimer, reloadTimer;
    protected float reload;
    protected float bulletSpeed;
    protected float dispersion;
    protected int health;
    private bool isDead;
    protected Vector3 TargetDir;
    private bool leadPlayer;
    protected float leadChance;
    
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
    public void incapacitate(float time) {}

    protected virtual void destruction() {
        Destroy(gameObject);
    }

    protected bool checkIfPlayerDetected() {
        if (playerRB == null) {
            return false;
        }
        return Vector2.Distance(new Vector2(rb.position.x, rb.position.z),
                    new Vector2(playerRB.position.x, playerRB.position.z)) < activeRadius &&
                Mathf.Abs(rb.position.y - playerRB.position.y) < 1 &&
                lineOfSightCheck();
    }
    protected bool lineOfSightCheck() {
        if (playerRB == null) {
            return false;
        }
        return !Physics.Raycast(playerRB.position, rb.position - playerRB.position, 
                    Vector3.Distance(rb.position, playerRB.position), 1 << 3);
    }
    
    protected bool isAimed() {
        if (leadPlayer) {
            return Vector3.Dot(TargetDir, gun.transform.right) > 0.95f;
        }
        return Vector3.Dot(TargetDir, gun.transform.right) > 0.965f;
    }

    protected IEnumerator idleTurn() {
        while (true) {
            yield return new WaitForSeconds(3);
            if (state == EnemyState.Idle) {
                if (Vector3.Dot(gun.transform.forward, transform.right) > 0.4f) {
                    cTurn = -12;
                } else if (Vector3.Dot(gun.transform.forward, transform.right) < -0.4f) {
                    cTurn = 12;
                } else {
                    cTurn = (Random.value < 0.3f) ? 12 : ((Random.value < 0.42f) ? -12 : 0);
                }
            }
        }
    }
    protected float TimerF(float val)
    {
        if (val > 0)
        {
            val -= Time.deltaTime;
            if (val <= 0) val = 0;
        }
        return val;
    }
    protected void turnTurret() {
        if (playerRB == null) {
            return;
        }
        if (leadPlayer && MyMath.InterceptDirection(playerRB.position, rb.position, playerRB.velocity, bulletSpeed, out Vector3 result)){
            TargetDir = result;
        } else TargetDir = (playerRB.position - rb.position).normalized;
        float dir = Vector3.Dot(gun.transform.forward, TargetDir);
        if (dir > 0.03f){
            cTurn = Mathf.Max(-rotSpeed, cTurn - 900 * Time.fixedDeltaTime);
        } else if (dir < -0.03f) {
            cTurn = Mathf.Min(rotSpeed, cTurn + 900 * Time.fixedDeltaTime);
        } else {
            cTurn = 0;
        }
    }

    protected void fire() {
        GameObject bullet = Object.Instantiate(bulletPrefab, gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(dispersion * (Random.value - 0.5f), Vector3.up)
         * (gun.transform.right * bulletSpeed);
        leadPlayer = Random.value < leadChance;
    }

}

public enum EnemyState {
    Start,
    Idle,
    Alert
}

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



    public bool hasLineOfSight, aimReady;
    public EnemyState state;
    protected float activeRadius;
    protected float rotSpeed, cTurn;
    protected float checkTimer, reloadTimer;
    protected float reload;
    protected float bulletSpeed;
    protected float dispersion;
    protected int health;
    protected Vector3 TargetDir;
    
    public void takeDamage(int dmg) {
        health -= dmg;
        if (health <= 0) {
            destruction();
        } 
    }

    protected virtual void destruction() {
        Destroy(gameObject);
    }

    protected bool playerCheck() {
        return Vector2.Distance(new Vector2(rb.position.x, rb.position.z),
                    new Vector2(playerRB.position.x, playerRB.position.z)) < activeRadius &&
                Mathf.Abs(rb.position.y - playerRB.position.y) < 1 &&
                lineOfSightCheck();
    }
    protected bool lineOfSightCheck() {
        return !Physics.Raycast(playerRB.position, rb.position - playerRB.position, 
                    Vector3.Distance(rb.position, playerRB.position), 1 << 3);
    }
    
    protected bool isAimed() {
        return Vector3.Dot((playerRB.position - rb.position).normalized, 
            gun.transform.right) > 0.96f;
    }

    protected IEnumerator idleTurn() {
        while (true) {
            yield return new WaitForSeconds(3);
            if (state == EnemyState.Idle) {
                cTurn = (Random.value > 0.7f) ? 12 : ((Random.value > 0.45f) ? -12 : 0);
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

    protected void fire() {
        GameObject bullet = Object.Instantiate(bulletPrefab, gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(dispersion * (Random.value - 0.5f), Vector3.up)
         * (gun.transform.right * bulletSpeed);
    }

}

public enum EnemyState {
    Idle,
    Alert
}

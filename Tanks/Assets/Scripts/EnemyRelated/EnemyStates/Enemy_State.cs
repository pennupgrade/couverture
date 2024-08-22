using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy_State
{
    protected Enemy enemy;
    
    public Enemy_State(Enemy enemy) {
        this.enemy = enemy;
    }

    public abstract Enemy_State Patrol(Vector3 playerPos);
    public abstract Enemy_State RotateTurret(Vector3 playerPos);
    public abstract Enemy_State Shoot(Vector3 playerPos);

    public virtual Enemy_State Move(Vector3 playerPos) {
        return this;
    }
    
    protected void removeCoroutine(Coroutine c) {
        if (c != null) {
            enemy.StopCoroutine(c);
            c = null;
        }
    }

    //--------------------helper functions for patrol, aiming, shooting----------------------

    protected virtual IEnumerator idleTurretTurn() {
        while (true) {
            yield return new WaitForSeconds(3);
            if (Vector3.Dot(enemy.gun.transform.forward, enemy.transform.right) > 0.6f) {
                enemy.cTurretTurn = -12;
            } else if (Vector3.Dot(enemy.gun.transform.forward, enemy.transform.right) < -0.6f) {
                enemy.cTurretTurn = 12;
            } else {
                float r = Random.value;
                if (Mathf.Abs(enemy.cTurretTurn) > 0) {
                    enemy.cTurretTurn = 0;
                } else if (r < 0.4f) {
                    enemy.cTurretTurn = 10;
                } else if (r < 0.8f) {
                    enemy.cTurretTurn = -10;
                } else {
                    enemy.cTurretTurn = 0;
                }
            }

        }
    }
    protected bool checkIfPlayerDetected(bool useFOV) {
        if (enemy.playerRB == null) {
            return false;
        }
        //ensures player is not above or below
        if (Mathf.Abs(enemy.rb.position.y - enemy.playerRB.position.y) > 1.4f) {
            return false;
        }
        //if distance under 5, only check if in LOS
        //otherwise also check if it is in the FOV triangle
        float d = Vector2.Distance(new Vector2(enemy.rb.position.x, enemy.rb.position.z),
            new Vector2(enemy.playerRB.position.x, enemy.playerRB.position.z));
        if (!useFOV) {
            return d < enemy.sightRange && lineOfSightCheck();
        } else if (d > 5) {
            Vector3 gunDirection = enemy.gun.transform.right;
            Vector3 gunToPlayer = (enemy.playerRB.position - enemy.gun.transform.position).normalized;
            return d < enemy.sightRange && Vector3.Dot(gunDirection, gunToPlayer) > (1 - enemy.FOV) && lineOfSightCheck();
        } else {
            return lineOfSightCheck();
        }
    }
    protected float getDist() {
        return Vector3.Distance(enemy.rb.position, enemy.playerRB.position);
    }
    protected bool lineOfSightCheck() {
        if (enemy.playerRB == null) {
            return false;
        }
        return !Physics.Raycast(enemy.playerRB.position, enemy.rb.position - enemy.playerRB.position, 
                    getDist(), 1 << 3);
    }
    protected bool isAimed() {
        return Vector3.Dot(enemy.TargetDir, enemy.gun.transform.right) > 0.975f;
    }
    protected void turnTurretTowardPlayer(bool leadPlayer) {
        if (enemy.playerRB == null) {
            return;
        }
        if (leadPlayer && MyMath.InterceptDirection(enemy.playerRB.position, enemy.rb.position, enemy.pTank.Velocity, enemy.bulletSpeed, out Vector3 result)){
            enemy.TargetDir = result;
        } else enemy.TargetDir = (enemy.playerRB.position - enemy.rb.position).normalized;

        float dir = Vector3.Dot(enemy.gun.transform.forward, enemy.TargetDir);
        if (dir > 0.02f){
            enemy.cTurretTurn = Mathf.Max(-enemy.rotSpeed, enemy.cTurretTurn - 900 * Time.deltaTime);
        } else if (dir < -0.02f) {
            enemy.cTurretTurn = Mathf.Min(enemy.rotSpeed, enemy.cTurretTurn + 900 * Time.deltaTime);
        } else {
            if (enemy.cTurretTurn > 0.0001f){
                enemy.cTurretTurn /= 1.4f;
            } else {
                enemy.cTurretTurn = 0;
            }
        }
    }
    protected void fire(float dispersion) {
        GameObject bullet = Object.Instantiate(enemy.bulletPrefab, enemy.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(dispersion * (Random.value - 0.5f), Vector3.up)
         * (enemy.gun.transform.right * enemy.bulletSpeed);
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
    }

    //------------------helper functions for movement--------------------------------

    protected Vector3 getRandomPoint(float radius) {
        for (int i = 0; i < 18; i++)
        {
            Vector3 randomPoint = enemy.transform.position + UnityEngine.Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                if (Mathf.Abs(hit.position.y - enemy.transform.position.y) < 1.2f) {
                    return hit.position;
                }
            }
        }
        return Vector3.zero;
    }
    protected bool hasReachedDest() {
        return Vector2.Distance(new Vector2(enemy.rb.position.x, enemy.rb.position.z),
                    new Vector2(enemy.destination.x, enemy.destination.z)) < 1;
    }
    protected void turnTowardsVector(Vector3 v) {
        float dir = Vector3.Dot(enemy.transform.forward, v);
        if (dir > 0.03f) {
            enemy.transform.eulerAngles -= enemy.turnSpeed * Time.fixedDeltaTime * Vector3.up; 
            enemy.gun.transform.eulerAngles += 0.5f * enemy.turnSpeed * Time.fixedDeltaTime * Vector3.up; 
        } else if (dir < -0.03f) {
            enemy.transform.eulerAngles += enemy.turnSpeed * Time.fixedDeltaTime * Vector3.up;
            enemy.gun.transform.eulerAngles -= 0.5f * enemy.turnSpeed * Time.fixedDeltaTime * Vector3.up;
        }
    }

}
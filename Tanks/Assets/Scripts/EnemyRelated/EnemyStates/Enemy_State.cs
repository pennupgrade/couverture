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
    protected virtual IEnumerator idleTurretTurnOmni() {
        while (true) {
            yield return new WaitForSeconds(2.5f);
            float dot = Vector3.Dot(enemy.gun.transform.right, enemy.transform.right);
            if (!((EnemyOmniMove)enemy).backwards && dot < -0.6f) {
                enemy.cTurretTurn = -30;
            } else if (((EnemyOmniMove)enemy).backwards && dot > 0.6f) {
                enemy.cTurretTurn = 30;
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
        //if distance under 4, only check if in LOS
        //otherwise also check if it is in the FOV triangle
        float d = Vector2.Distance(new Vector2(enemy.rb.position.x, enemy.rb.position.z),
            new Vector2(enemy.playerRB.position.x, enemy.playerRB.position.z));
        if (!useFOV) {
            return d < enemy.sightRange && lineOfSightCheck();
        } else if (d > 4) {
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
        return !Physics.Raycast(enemy.playerRB.position + 0.2f * Vector3.up, 
            enemy.rb.position - (enemy.playerRB.position + 0.2f * Vector3.up), getDist(), 1 << 3);
    }
    protected bool checkFriendlyFire(float dist) {
        return !Physics.Raycast(enemy.gunShotPos.position + 0.3f * enemy.gun.transform.right,
            enemy.playerRB.position - enemy.rb.position, dist, 1 << 8);
    }
    protected bool isAimed() {
        return Vector3.Dot(enemy.TargetDir, enemy.gun.transform.right) > 0.975f;
    }
    protected void turnTurretTowardPlayer(bool leadPlayer) {
        if (enemy.playerRB == null) {
            return;
        }
        if (leadPlayer && MyMath.InterceptDirection(enemy.playerRB.position, enemy.rb.position, enemy.pTank.Velocity, 
                enemy.bulletSpeed, out Vector3 result)){
            enemy.TargetDir = result;
        } else enemy.TargetDir = (enemy.playerRB.position - enemy.rb.position).normalized;

        turnTurretVecMath();
    }
    protected void turnTurretTowardPlayerTimeDelay(bool leadPlayer, float duration) {
        if (enemy.playerRB == null) {
            return;
        }
        if (leadPlayer){
            enemy.TargetDir = ((enemy.playerRB.position + enemy.pTank.Velocity * duration) - enemy.rb.position).normalized;
        } else {
            enemy.TargetDir = (enemy.playerRB.position - enemy.rb.position).normalized;
        }

        turnTurretVecMath();
    }
    protected void turnTurretVecMath() {
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
    protected void fire(float dispersion, bool random = true) {
        GameObject bullet = Object.Instantiate(enemy.bulletPrefab, enemy.gunShotPos.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(dispersion * ((random) ? (Random.value - 0.5f) : 1), Vector3.up)
         * (enemy.gun.transform.right * bullet.GetComponent<Projectile>().bulletSpeed);
        bullet.transform.rotation = Quaternion.LookRotation(bullet.GetComponent<Rigidbody>().velocity);
    }

    //------------------helper functions for movement--------------------------------

    protected Vector3 getRandomPoint(float radius) {
        for (int i = 0; i < 18; i++)
        {
            Vector3 randomPoint = enemy.transform.position + radius * UnityEngine.Random.insideUnitSphere;
            randomPoint.y = enemy.rb.position.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                if (Mathf.Abs(hit.position.y - enemy.rb.position.y) < 1.2f) {
                    return hit.position;
                }
            }
        }
        return Vector3.zero;
    }
    protected Vector3 getLOSPoint(Vector3 pos, float radius, float avoidRadius) {
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomPoint = pos + radius * UnityEngine.Random.insideUnitSphere;
            randomPoint.y = enemy.rb.position.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                Vector3 newPos = new Vector3(hit.position.x, enemy.rb.position.y, hit.position.z);
                //Debug.DrawRay(newPos, enemy.playerRB.position + 0.2f * Vector3.up - newPos, Color.green, 2);
                if (Mathf.Abs(hit.position.y - enemy.rb.position.y) < 1.2f &&
                    !Physics.Raycast(newPos, (enemy.playerRB.position + 0.2f * Vector3.up) - newPos, 
                    Vector3.Distance(newPos, (enemy.playerRB.position + 0.2f * Vector3.up)), 1 << 3) &&
                    Vector3.Distance(enemy.player.transform.position, newPos) > avoidRadius) {

                    return hit.position;
                }
            }
        }
        return getPlayerPoint(2);
    }
    
    protected Vector3 getRandomHidePoint(float radius) {
        for (int i = 0; i < 18; i++)
        {
            Vector3 randomPoint = enemy.transform.position + radius * UnityEngine.Random.insideUnitSphere;
            randomPoint.y = enemy.rb.position.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                Vector3 newPos = new Vector3(hit.position.x, enemy.rb.position.y, hit.position.z);
                if (Mathf.Abs(hit.position.y - enemy.rb.position.y) < 1.2f && 
                    Physics.Raycast(newPos, (enemy.playerRB.position  + 0.2f * Vector3.up) - newPos, 
                    Vector3.Distance(newPos, enemy.playerRB.position + 0.2f * Vector3.up), 1 << 3)) {
                    return hit.position;
                }
            }
        }
        return getRandomPoint(radius);
    }
    protected Vector3 getPlayerPoint(float radius) {
        if (enemy.playerRB == null || Mathf.Abs(enemy.playerRB.position.y - enemy.rb.position.y) > 1.2f) {
            return getRandomPoint(6);
        }
        for (int i = 0; i < 10; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(enemy.playerRB.position + radius * UnityEngine.Random.insideUnitSphere,
                 out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return getRandomPoint(radius);
    }
    protected Vector3 getRandomNavPoint(Vector3 point, float radius) {
        for (int i = 0; i < 18; i++)
        {
            Vector3 randomPoint = point + radius * UnityEngine.Random.insideUnitSphere;
            randomPoint.y = enemy.rb.position.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                if (Mathf.Abs(hit.position.y - enemy.transform.position.y) < 1.2f) {
                    return hit.position;
                }
            }
        }
        return getRandomPoint(radius);
    }
    protected Vector3 getRandomNavPointAwayFromPlayer(Vector3 point, float radius, float avoidRadius) {
        Vector3 p;
        int i = 0;
        do {
            p = getRandomNavPoint(point, radius);
            i++;
        } while (Vector2.Distance(new Vector2(enemy.playerRB.position.x, enemy.playerRB.position.z),
        new Vector2(p.x, p.z)) < avoidRadius && i < 8);
        return p;
    }
    protected bool hasReachedDest() {
        return Vector2.Distance(new Vector2(enemy.rb.position.x, enemy.rb.position.z),
                    new Vector2(enemy.destination.x, enemy.destination.z)) < 1;
    }
    protected void turnTowardsVector(Vector3 v, float accel) {
        float dir = Vector3.Dot(enemy.transform.forward, v);
        if (dir > 0.03f) {
            if (enemy.cTurnSpeed > 0) {
                enemy.cTurnSpeed -= 2 * accel * Time.fixedDeltaTime;
            } else {
                enemy.cTurnSpeed = Mathf.Max(-enemy.turnSpeed, enemy.cTurnSpeed - accel * Time.fixedDeltaTime);
            }
        } else if (dir < -0.03f) {
            if (enemy.cTurnSpeed < 0) {
                enemy.cTurnSpeed += 2 * accel * Time.fixedDeltaTime;
            } else {
                enemy.cTurnSpeed = Mathf.Min(enemy.turnSpeed, enemy.cTurnSpeed + accel * Time.fixedDeltaTime);
            }
        } else {
            if (enemy.cTurnSpeed < 20) {
                enemy.cTurnSpeed = 0;
            } else {
                enemy.cTurnSpeed /= 1.2f;
            }
        }
    }
    //------------------helper functions for omni movement--------------------------------

    protected void turnTowardsVectorOmni(Vector3 v, float accel) {
        ((EnemyOmniMove)enemy).backwards = Vector3.Dot(enemy.transform.right, v) < 0;
        float dir = Vector3.Dot(enemy.transform.forward, v);
        if ((dir > 0.03f && !((EnemyOmniMove)enemy).backwards) || (dir < -0.03f && ((EnemyOmniMove)enemy).backwards)) {
            if (enemy.cTurnSpeed > 0) {
                enemy.cTurnSpeed -= 2 * accel * Time.fixedDeltaTime;
            } else {
                enemy.cTurnSpeed = Mathf.Max(-enemy.turnSpeed, enemy.cTurnSpeed - accel * Time.fixedDeltaTime);
            }
        } else if ((dir < -0.03f && !((EnemyOmniMove)enemy).backwards) || (dir > 0.03f && ((EnemyOmniMove)enemy).backwards)) {
            if (enemy.cTurnSpeed < 0) {
                enemy.cTurnSpeed += 2 * accel * Time.fixedDeltaTime;
            } else {
                enemy.cTurnSpeed = Mathf.Min(enemy.turnSpeed, enemy.cTurnSpeed + accel * Time.fixedDeltaTime);
            }
        } else {
            if (enemy.cTurnSpeed < 20) {
                enemy.cTurnSpeed = 0;
            } else {
                enemy.cTurnSpeed /= 1.2f;
            }
        }
    }
}
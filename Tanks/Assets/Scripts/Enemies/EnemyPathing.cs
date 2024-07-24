using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathing : Enemy
{
    protected float speed, turnSpeed;
    protected float waypointTimer;
    [SerializeField] protected bool stopMovement;
    protected Vector3 destination;
    protected NavMeshAgent agent;

    protected Vector3 getRandomPoint(float radius) {
        for (int i = 0; i < 18; i++)
        {
            Vector3 randomPoint = rb.position + Random.insideUnitSphere * radius * 0.5f;
            randomPoint += randomPoint - rb.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return Vector3.zero;
    }
    protected void agentSetup() {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    protected IEnumerator calcPath1() {
        yield return new WaitForSeconds(0.1f + 0.1f * Random.value);
        while (true) {
            agent.SetDestination(destination);
            yield return new WaitForSeconds(3);
        }
    }
    protected IEnumerator stopMove() {
        stopMovement = true;
        yield return new WaitForSeconds(1.3f);
        stopMovement = false;
    }
    protected void turnTowardsPath() {
        float dir = Vector3.Dot(transform.forward, agent.desiredVelocity);
        if (dir > 0.05f) {
            transform.eulerAngles -= turnSpeed * Time.fixedDeltaTime * Vector3.up; 
            gun.transform.eulerAngles += 0.5f * turnSpeed * Time.fixedDeltaTime * Vector3.up; 
        } else if (dir < -0.05f) {
            transform.eulerAngles += turnSpeed * Time.fixedDeltaTime * Vector3.up;
            gun.transform.eulerAngles -= 0.5f * turnSpeed * Time.fixedDeltaTime * Vector3.up;
        }
    }
    protected bool hasReachedDest() {
        return Vector2.Distance(new Vector2(rb.position.x, rb.position.z),
                    new Vector2(destination.x, destination.z)) < 1;
    }

}

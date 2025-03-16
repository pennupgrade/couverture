using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private GameObject player;
    [SerializeField] private float moveSpeed;
    private float currMoveSpeed;
    private float stateStart;
    private float stateDur;
    [SerializeField] private float maxRange;
    [SerializeField] private Transform plugBase;
    [SerializeField] private float guardRange;
    [SerializeField] private float turnSpeed;

    public enum bmState {
        Idle,
        Guarding,
        Chasing,
        Attacking,  
        Rage
    }
    private float dt;  
    public bmState moveState;
    public Boss boss;
    public BossStateMachine bossState;
    public NavMeshAgent agent; 
    public float lastDestTime;
    public float destCD;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {   
        if (boss.isUnplugged()) {
            moveSpeed = 0.85f;
            agent.speed = 0.85f;
        }

        if (bossState.currentAttack != BossStateMachine.Attack.Charge) {
            Vector3 newtarget = player.transform.position;
            newtarget.y = transform.position.y;
            transform.LookAt(newtarget);
            currMoveSpeed = moveSpeed;
            agent.enabled = true;
            agent.speed = moveSpeed;
        } else {
            agent.enabled = false;
            currMoveSpeed = 0f;
        }
        
        Vector3 bossToPlayer = new Vector3(player.transform.position.x - this.transform.position.x, 
                            0, player.transform.position.z - this.transform.position.z);

        Vector3 bossToPlug = new Vector3(transform.position.x - plugBase.position.x, 0,
                            transform.position.z - plugBase.position.z);

        Vector3 playerToPlug =  new Vector3(player.transform.position.x - plugBase.position.x, 
                            0, player.transform.position.z - plugBase.position.z);
        bossToPlayer.Normalize();
        // float angle = Mathf.Acos(Vector3.Dot(dir, new Vector3(0, 0, 1)));
        // Debug.Log(angle);
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle, 0), 1.0f);
        
        if (Time.time > stateStart + stateDur) {
            int r = UnityEngine.Random.Range(0,2);
            print("Switch to" + r);
            if (r == 0) {
                moveState = bmState.Guarding;
            } else { 
                moveState = bmState.Chasing;
            }
            stateStart = Time.time;
            stateDur = UnityEngine.Random.Range(2,10);
        }

        if (Time.time > lastDestTime + destCD && bossState.currentAttack != BossStateMachine.Attack.Charge) {
            if ((this.transform.position - plugBase.position).magnitude > maxRange -1 && !boss.isUnplugged()) {
                Retreat();
            } else {
                if (moveState == bmState.Guarding && !boss.isUnplugged()) {
                    Guard();
                } else {
                    Chase();
                } 
            }
            lastDestTime = Time.time;
        }
       
    }

    public void Guard() {
        Vector3 dir = new Vector3(player.transform.position.x - this.transform.position.x, 
                            0, player.transform.position.z - this.transform.position.z);
        Vector3 plugDir = player.transform.position + plugBase.transform.position;
        if ((plugDir - plugBase.position).magnitude > guardRange) {
            agent.destination = dir.normalized * guardRange + plugBase.position;
        } else {
            agent.destination = plugDir/2;
        }
        
        // Vector3 crossedWith = new Vector3(0, 1, 0);
        // Vector3 lr = Vector3.Cross(plugDir, crossedWith);
        // dir.Normalize();
        // dir = (lr * Vector3.Dot(dir, lr)).normalized;
        // if (rb.GetAccumulatedForce().magnitude < 25 && rb.velocity.magnitude < currMoveSpeed) {
        //     rb.velocity = dir * currMoveSpeed;
        // }
    }
    public void Chase() {
        if (boss.isUnplugged()) {
            agent.destination = player.transform.position;
        } else { 
            Vector3 dir = player.transform.position - plugBase.position;
            if (dir.magnitude < maxRange) {
                agent.destination = player.transform.position;
            } else {
                agent.destination = plugBase.position + dir.normalized * (maxRange - 1);
            }
        }           
    }
    public void Retreat() {
        Vector3 dir = player.transform.position - plugBase.transform.position;
        agent.destination = plugBase.transform.position + dir.normalized * (maxRange - 1);
        
        // dir.Normalize();
        // if (rb.GetAccumulatedForce().magnitude < 25 && rb.velocity.magnitude < currMoveSpeed) {
        //     rb.AddForce(dir * 3f, ForceMode.VelocityChange);
        // }
    }

    // public void Charge() {
    //     if (Time.time > chargeStartTime + 1f) {
    //         print(chargeDir);
    //         rb.velocity = chargeSpeed * transform.forward; 
    //     }
    //     //
    //     if ((transform.position - chargeStart).magnitude > chargeRange || Time.time > chargeStartTime + 2f) { 
    //         this.moveState = bmState.Idle;
    //     }
    // }
}

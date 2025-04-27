using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public Vector3 offset;

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
    public GameObject top;
    public float destCD;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {   
        top.transform.position = this.transform.position + offset;
        if (boss.isUnplugged()) {
            moveSpeed = 0.85f;
            agent.speed = 0.85f;
        }
        if (bossState.currentAttack != BossStateMachine.Attack.Charge) {
            top.transform.SetParent(this.transform.parent);
            Vector3 direction = player.transform.position - this.transform.position;
            direction.y = 0;
            top.transform.rotation = Quaternion.LookRotation(direction);
            currMoveSpeed = moveSpeed;
            agent.enabled = true;
            agent.speed = moveSpeed;
        } else {
            top.transform.parent = this.transform;
            agent.enabled = false;
            currMoveSpeed = 0f;
        }
        
        Vector3 bossToPlayer = new Vector3(player.transform.position.x - this.transform.position.x, 
                            0, player.transform.position.z - this.transform.position.z);
        bossToPlayer.Normalize();   
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
    }

    public void SetPlayer(GameObject g) { 
        player = g;
    }

}

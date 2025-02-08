using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private GameObject player;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float chargeCD;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeRange;
    private Vector3 chargeDir;
    private Vector3 chargeStart;
    private float chargeStartTime;

    private float stateTimer;
    [SerializeField] private GameObject indicatorObject;
    [SerializeField] private Transform plugBase;
    [SerializeField] private float guardRange;
    [SerializeField] private float turnSpeed;

    public enum bmState {
        Idle,
        Charging, 
        Chasing, 
        Stopped,
        Rage
    }
    private float dt;  
    public bmState moveState;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
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

        if (moveState == bmState.Charging) {
            Charge();
        } else {
            if (playerToPlug.magnitude > guardRange) { //if Player is outside of GuardRange
                // if (bossToPlug.magnitude >= guardRange) {
                //     Debug.Log("Guarding");
                //     Chase();
                // } else {
                //     Debug.Log("Chasing");
                //     Chase();
                // }
                Guard();
            } else {
                Debug.Log("Chasing");
                Chase();
            }
        }
        //terms for acceleration
        
        
        //rb.velocity = dir * moveSpeed; 
        if (Time.time > chargeStartTime + chargeCD) {
            chargeStart = this.transform.position;
            chargeStartTime = Time.time;

            Vector3 chargeDir = player.transform.position - transform.position;
            chargeDir.Normalize();
            chargeDir.y = 0;
            Vector3 indicatorLoc = this.transform.position + chargeDir * chargeRange/2;
            Vector3 rot = Quaternion.LookRotation(chargeDir).eulerAngles;
            rot.x = -90;
            Instantiate(indicatorObject, indicatorLoc, Quaternion.Euler(rot));
            moveState = bmState.Charging;
        }
    }

    public void Guard() {
        Vector3 newtarget = player.transform.position;
        newtarget.y = transform.position.y;
        transform.LookAt(newtarget);
        Vector3 dir = new Vector3(player.transform.position.x - this.transform.position.x, 
                            0, player.transform.position.z - this.transform.position.z);
        Vector3 plugDir = new Vector3(transform.position.x - plugBase.position.x, 0,
                            transform.position.z - plugBase.position.z);    
        Vector3 crossedWith = new Vector3(0, 1, 0);
        Vector3 lr = Vector3.Cross(plugDir, crossedWith);
        dir.Normalize();
        dir = (lr * Vector3.Dot(dir, lr)).normalized;
        if (rb.GetAccumulatedForce().magnitude < 25 && rb.velocity.magnitude < moveSpeed) {
            rb.velocity = dir * moveSpeed;
        }
    }
    public void Idle(){ 
        rb.velocity = new Vector3();
    }
    public void Chase() {
        Vector3 newtarget = player.transform.position;
        newtarget.y = transform.position.y;
        transform.LookAt(newtarget);
        if (rb.GetAccumulatedForce().magnitude < 25 && rb.velocity.magnitude < moveSpeed) {
            rb.velocity = transform.forward * moveSpeed;
        }
    }
    public void Retreat() {
        Vector3 dir = new Vector3(plugBase.position.x - this.transform.position.x, 
                            0, plugBase.position.z - this.transform.position.z);
        dir.Normalize();
        if (rb.GetAccumulatedForce().magnitude < 25 && rb.velocity.magnitude < moveSpeed) {
            rb.AddForce(dir * 3f, ForceMode.VelocityChange);
        }
    }

    public void Charge() {
        if (Time.time > chargeStartTime + 1f) {
            print(chargeDir);
            rb.velocity = chargeSpeed * transform.forward; 
        }
        //
        if ((transform.position - chargeStart).magnitude > chargeRange || Time.time > chargeStartTime + 2f) { 
            this.moveState = bmState.Idle;
        }
    }
}

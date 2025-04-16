using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bomb : Enemy
{
    [HideInInspector] public float explosionRadius;
    public LayerMask explosionLM;
    public Material glowMat;
    private bool playerHit;
    void Awake() {
        enemyState = new Bomb_Start(this);
        playerHit = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        //set enemy values
        health = 200;
        sightRange = 8;
        FOV = 1.4f;
        speed = 1.5f;
        turnSpeed = 160;
        explosionRadius = 2.4f;
        
        damageFlash = new DamageFlash(transform.Find("body_control").gameObject); // I hate this so much
        findPlayer();
        agentSetup();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 playerPos = player.transform.position;
        enemyState = enemyState.Patrol(playerPos);
        //enemyState = enemyState.RotateTurret(playerPos);
        enemyState = enemyState.Shoot(playerPos);

        //gun.transform.eulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
    }
    void FixedUpdate() {
        if (playerRB == null) return;

        agent.nextPosition = transform.position;
        if (isStunned) return;
        //turning
        if (!stopTurns && moveStraightTimer == null) {
            enemyState = enemyState.Move(playerRB.position);
            transform.eulerAngles += cTurnSpeed * Time.fixedDeltaTime * Vector3.up;
        }
        //moving
        transform.position += cSpeed * Time.fixedDeltaTime * transform.forward;
    }
    void OnCollisionEnter(Collision collision) {
        if ((collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Tank")
             && cSpeed > 0.01f){
            StartCoroutine(stopMove(0.5f));
        }
    }
    
    protected override void destruction() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLM);
        foreach (var hit in hitColliders) {
            if (hit.gameObject != this.gameObject && hit.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d)) {
                if (gameObject.tag == "Enemy") {
                    d.takeDamage(50);
                } else if (!playerHit) {
                    d.takeDamage(100);
                    playerHit = true;
                }
            }
        }

        base.destruction();
    }
    
    public void boom() {
        die();
    }

    public void turnOnLight() {
        glowMat.SetFloat("_Pulsing", 1);
    }
}

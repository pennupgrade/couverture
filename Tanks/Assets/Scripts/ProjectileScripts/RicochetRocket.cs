using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetRocket : Projectile
{
    [SerializeField] private int bounces;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private int wallTouchCounter;
    
    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        dontDamageOnSpawnDelay = 0;
        bulletSpeed = 6f;
        wallTouchCounter = 0;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<MeshRenderer>().material.SetFloat("_Glowy", 0.1f);
    }

    void FixedUpdate () {
        lastVelocity = rb.velocity;
    }

    private void ReflectBullet(Vector3 bulletDir, Vector3 wallNormal)
    {
        Vector3 bounceDirection = Vector3.Reflect(bulletDir, wallNormal);
        rb.velocity = bounceDirection * lastVelocity.magnitude;
        transform.rotation = Quaternion.LookRotation(rb.velocity);
        bounces--;
        if (bounces < 0)
        {
            destruction();
        } else {
            audioManager.Play("Bounce");
        }
    }
    public void reduceBounces()
    {
        bounces--;
    }

    void OnCollisionEnter(Collision collision) {
        if (defaultCollisionChecks(collision)) return;
        
        Vector3 wallNormal = collision.contacts[0].normal;
        Vector3 bulletDir = lastVelocity.normalized;
        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Untagged")
        {
            ReflectBullet(bulletDir, wallNormal);
            return;
        }

        if (collision.gameObject.tag == "OneWay")
        {

            if (Vector3.Dot(bulletDir, wallNormal) > 0) // Angle check to see if bullet is behind wall
            {
                return;
            }

            ReflectBullet(bulletDir, wallNormal);
        }
    }
    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Untagged")
        {
            wallTouchCounter++;
            if (wallTouchCounter > 10) destruction();
        }
    }
}

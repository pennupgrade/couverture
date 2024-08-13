using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Default : Projectile
{
    [SerializeField] private int bounces;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        lastVelocity = rb.velocity;
    }

    private void ReflectBullet(Vector3 bulletDir, Vector3 wallNormal)
    {
        Vector3 bounceDirection = Vector3.Reflect(bulletDir, wallNormal);
        rb.velocity = bounceDirection * lastVelocity.magnitude;
        bounces--;
        damage += 300;
        if (bounces < 0) // changed from == -1 in case... something weird happens
        {
            destruction();
        }
        return;
    }

    void OnCollisionEnter(Collision collision) {
        // Refactored this a bit -- Kevin 8/11
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
}

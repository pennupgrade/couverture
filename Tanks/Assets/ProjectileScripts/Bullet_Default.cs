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
        damage = 100;
        lifetime = 12;
        bounces = 1;
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
        if (bounces < 0) // changed from == -1 in case... something weird happens
        {
            destruction();
        }
        return;
    }

    void OnCollisionEnter(Collision collision) {
        // Refactored this a bit -- Anthony 8/10
        Vector3 wallNormal = collision.contacts[0].normal;
        Vector3 bulletDir = lastVelocity.normalized;

        if (collision.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d)) // hit a player
        {
            d.takeDamage(damage);
            destruction();
            return;
        }

        if (collision.gameObject.tag == "Projectile") // Parry other projectile
        {
            destruction();
        }

        if (collision.gameObject.tag == "OneWay")
        {
            Debug.Log(Vector3.Dot(bulletDir, wallNormal));

            if (Vector3.Dot(bulletDir, wallNormal) > 0) // Angle check to see if bullet is behind wall
            {
                return;
            }

            ReflectBullet(bulletDir, wallNormal);
        }

        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Untagged")
        {
            ReflectBullet(bulletDir, wallNormal);
        }
    }
    public override void destruction() {
        Destroy(gameObject);
    }
}

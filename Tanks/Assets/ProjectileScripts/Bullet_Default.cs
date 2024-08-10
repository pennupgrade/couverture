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

    void OnCollisionEnter(Collision collision) {
        //Debug.Log("hit");
        if (collision.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d)) {
            d.takeDamage(damage);
            destruction();
        } else if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Untagged"){
            Vector3 bounceDirection = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            rb.velocity = bounceDirection * lastVelocity.magnitude;
            bounces--;
            if (bounces == -1) {
                destruction();
            }
        } else if (collision.gameObject.tag == "Projectile"){
            destruction();
        }
    }
    public override void destruction() {
        Destroy(gameObject);
    }
}

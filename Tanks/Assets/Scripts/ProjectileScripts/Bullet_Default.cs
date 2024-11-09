using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Default : Projectile
{
    private bool changeWhenBounce;
    [SerializeField] private int bounces;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private Material material;
    
    // Start is called before the first frame update
    void Awake() {
        bulletSpeed = 3;
    }
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
        if (bounces <= 0) // changed from == -1 in case... something weird happens
        {
            destruction();
        } else if (changeWhenBounce)
        {
            material.SetFloat("_Glowy", 1);
            damage *= 3;
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        bounces--;
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
    public void addBounceChange() {
        changeWhenBounce = true;
        material = GetComponent<MeshRenderer>().material;
    }

    protected override void removeObjectFromGame()
    {
        GetComponent<Animator>().Play("DefaultBulletFadeOut");
        rb.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        this.enabled = false;

        Destroy(gameObject, 0.25f);
    }
}

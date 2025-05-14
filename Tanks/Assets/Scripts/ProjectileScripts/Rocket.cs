using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile
{
    [SerializeField] private float spinSpeed;
    private Rigidbody rb;
    
    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        bulletSpeed = 5f;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void damageIncrease() {
        damage = 600;
        GetComponent<MeshRenderer>().material.SetInt("_Player", 1);
        GetComponent<MeshRenderer>().material.SetFloat("_Glowy", 1);
    }

    // Update is called once per frame
    void FixedUpdate() {
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, 0, spinSpeed) * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    void OnCollisionEnter(Collision collision) {
        if (defaultCollisionChecks(collision, damage >= 600)) return;

        if (damage >= 600) {
            GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        }

        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Untagged")
        {
            destruction();
            return;
        }
        
        Vector3 wallNormal = collision.contacts[0].normal;
        Vector3 bulletDir = rb.velocity.normalized;

        if (collision.gameObject.tag == "OneWay")
        {
            if (Vector3.Dot(bulletDir, wallNormal) > 0) // Angle check to see if bullet is behind wall
            {
                return;
            }

            destruction();
        }
    }

    protected override void removeObjectFromGame()
    {
        GetComponent<Animator>().Play("DefaultBulletFadeOut");
        rb.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
        audioManager.Stop("Rocket");

        Destroy(gameObject, 0.25f);
    }
}

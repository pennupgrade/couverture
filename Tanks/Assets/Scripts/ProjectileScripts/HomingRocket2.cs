using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingRocket2 : Projectile
{
    public GameObject player;
    private Rigidbody rb;

    private bool disabled;

    private float homingStr, Cturn;
    
    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        disabled = true;
        bulletSpeed = 4f;
        homingStr = 30;
        Cturn = 0;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) Destroy(gameObject);

        if (player == null) return;

        if (disabled && (Vector3.Distance(player.transform.position, transform.position) < 2.8f || Vector3.Dot(player.transform.position - transform.position, transform.forward) < -0.1f)) {
            homingStr = 180;
            disabled = false;
        } else if (!disabled) {
            bulletSpeed = Mathf.Max(bulletSpeed - 2 * Time.deltaTime, 3);
        }
        

        Vector3 v = player.transform.position - transform.position;
        float dot = Vector3.Dot(transform.right, (new Vector3(v.x, 0, v.z)).normalized);
        if (dot > 0.04f) {
            Cturn = homingStr;
        } else if (dot < -0.04f) {
            Cturn = -homingStr;
        } else {
            Cturn = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        transform.eulerAngles += Cturn * Time.fixedDeltaTime * Vector3.up;
        rb.MovePosition(rb.position + (Time.fixedDeltaTime * bulletSpeed * transform.forward));
    }

    void OnCollisionEnter(Collision collision) {
        if (defaultCollisionChecks(collision)) return;

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

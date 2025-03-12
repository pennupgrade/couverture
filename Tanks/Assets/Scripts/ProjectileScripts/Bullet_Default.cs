using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet_Default : Projectile
{
    private int originalDamage;
    private int startBounces;
    private bool changeWhenBounce;
    [SerializeField] private int bounces;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private Material material;
    private Animator animator;
    private Collider collider;
    public MeshTrail meshTrail;
    
    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        material = GetComponent<MeshRenderer>().material;
        startBounces = bounces;
        originalDamage = damage;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        lastVelocity = rb.velocity;
    }

    public void StartBullet() {
        collider.enabled = true;
        this.enabled = true;
        changeWhenBounce = false;
        material.SetFloat("_Glowy", 0);
        bulletSpeed = 3;
        damage = originalDamage;
        bounces = startBounces;
        lifetime = startLifetime;
        destroyed = false;
        meshTrail.StartTrail();
        animator.Play("DefaultBulletFadeIn");
        
    }

    public void OnSceneLoaded(Scene s, LoadSceneMode m) {
        if (m == LoadSceneMode.Single) {
            PoolManager.bulletPool.Release(this);
        }
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
        }

        if (!destroyed)
            transform.rotation = Quaternion.LookRotation(rb.velocity);

        bounces--;
    }

    void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.gameObject.tag);
        // if (collision.gameObject.tag == "NotARealCollider") {
        //     return;
        // }
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
    }

    protected override void removeObjectFromGame()
    {
        animator.Play("DefaultBulletFadeOut");
        rb.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        this.enabled = false;

        StartCoroutine(RemoveCoroutine());
    }

    private IEnumerator RemoveCoroutine() {
        yield return new WaitForSeconds(0.25f);
        PoolManager.bulletPool.Release(this);
    }
}

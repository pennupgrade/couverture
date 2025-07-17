// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System;
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
    private Collider theCollider;
    [NonSerialized] public MeshTrail meshTrail;
    private int wallTouchCounter;
    
    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        wallTouchCounter = 0;
        animator = GetComponent<Animator>();
        theCollider = GetComponent<Collider>();
        material = GetComponent<MeshRenderer>().material;
        meshTrail = GetComponent<MeshTrail>();

        if (meshTrail is null) {
            throw new InvalidOperationException("Meshtrail doesn't exist, BulletDefault being used incorrectly!");
        }
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
        theCollider.enabled = true;
        this.enabled = true;
        changeWhenBounce = false;
        material.SetFloat("_Glowy", 0);
        material.SetInt("_Player", 0);
        bulletSpeed = 3;
        wallTouchCounter = 0;
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
            return;
        } else if (changeWhenBounce)
        {
            material.SetFloat("_Glowy", 1);
            damage *= 5;
        }

        if (!destroyed) {
            audioManager.Play("Bounce");
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
    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Untagged")
        {
            wallTouchCounter++;
            if (wallTouchCounter > 10) destruction();
        }
    }
    public void addBounceChange() {
        changeWhenBounce = true;
        material.SetInt("_Player", 1);
    }

    public override void destruction() {
        if (destroyed) return;
        destroyed = true;
        if (explosionPrefab != null) {
            GameObject expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(expl, 2);
        }

        removeObjectFromGame();
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
        yield return new WaitForSeconds(0.16f);
        PoolManager.bulletPool.Release(this);
    }
}

// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] protected float lifetime;
    [SerializeField] protected GameObject explosionPrefab;
    [SerializeField] protected GameObject ricochetSoundPrefab;
    public float bulletSpeed;
    protected bool destroyed;
    public GameObject parent;

    public float dontDamageOnSpawnDelay = 0.03f;

    protected float startLifetime;

    public AudioManager audioManager;

    protected virtual void Awake() {
        startLifetime = lifetime;
    }

    protected virtual void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) removeObjectFromGame();
    }

    public virtual void destruction() {
        if (destroyed) return;
        destroyed = true;
        if (explosionPrefab != null) {
            var expl = Instantiate(explosionPrefab, transform.position - 0.06f * transform.forward,
                                   Quaternion.identity);
            Destroy(expl, 2);
        }

        //first child should be a trail
        if (transform.childCount != 0) {
            if (transform.GetChild(0).gameObject.TryGetComponent(out ParticleSystem ps)) {
                ps.Stop();

                //for rocket
                transform.GetChild(0).localScale = 1 / transform.localScale.x * transform.GetChild(0).localScale;
                Destroy(transform.GetChild(0).gameObject, 2.5f);
            }

            transform.GetChild(0).parent = null;
        }

        removeObjectFromGame();
    }

    protected virtual void removeObjectFromGame() {
        Destroy(gameObject);
    }

    protected bool defaultCollisionChecks(Collision collision, bool playerRocket = false) {
        if (collision.gameObject.TryGetComponent(out IDestroyable d)) // hit a player
        {
            if (startLifetime - lifetime > dontDamageOnSpawnDelay || parent != collision.gameObject) {
                if (!destroyed) {
                    var isRicochet = damage > 200;

                    var enemy = d as Enemy;

                    if (enemy && isRicochet && collision.gameObject.tag != "Player") {
                        enemy.setRicochet(true);
                        if (collision.gameObject.TryGetComponent(out ShieldedEnemy se)) {
                            if (!se.getShieldActivated()) {
                                var sound = Instantiate(ricochetSoundPrefab, transform.position, Quaternion.identity);
                                Destroy(sound, 2);
                            }
                        }
                        else {
                            var sound = Instantiate(ricochetSoundPrefab, transform.position, Quaternion.identity);
                            Destroy(sound, 2);
                        }
                    }

                    if (enemy != null && parent != null && parent.GetComponent<Tank>() != null) {
                        enemy.TakeDamageFromPlayer(damage);
                    } else {
                        d.takeDamage(damage);
                    }
                }

                destruction();
                return true;
            }
        }

        if (!playerRocket && collision.gameObject.tag == "Projectile" ||
            collision.gameObject.tag == "NoBounce") // Parry other projectile or no bounce
        {
            destruction();
            return true;
        }

        return false;
    }
}
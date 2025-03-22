using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] protected float lifetime;
    [SerializeField] protected GameObject explosionPrefab;
    public float bulletSpeed;
    protected bool destroyed;
    public GameObject parent;

    public float dontDamageOnSpawnDelay=0.1f;

    protected float startLifetime;

    protected virtual void Awake()
    {
        startLifetime = lifetime;
    }

    protected virtual void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) Destroy(gameObject);
    }
    public virtual void destruction() {
        if (destroyed) return;
        destroyed = true;
        if (explosionPrefab != null) {
            GameObject expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(expl, 2);
        }
        //first child should be a trail
        if (transform.childCount != 0) {
            if (transform.GetChild(0).gameObject.TryGetComponent<ParticleSystem>(out ParticleSystem ps)){
                ps.Stop();
                //for rocket
                transform.GetChild(0).localScale = 1 / transform.localScale.x * transform.GetChild(0).localScale;
                Destroy(transform.GetChild(0).gameObject, 2.5f);
            }
            transform.GetChild(0).parent = null;
        }

        removeObjectFromGame();
    }

    protected virtual void removeObjectFromGame()
    {
        Destroy(gameObject);
    }

    protected bool defaultCollisionChecks(Collision collision) {
        if (collision.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d) && (startLifetime-lifetime)>dontDamageOnSpawnDelay) // hit a player
        {
            d.takeDamage(damage);
            destruction();
            return true;
        }

        if (collision.gameObject.tag == "Projectile" || collision.gameObject.tag == "NoBounce") // Parry other projectile or no bounce
        {
            destruction();
            return true;
        }
        return false;
    }

}

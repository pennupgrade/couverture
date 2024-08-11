using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] protected float lifetime;
    //[SerializeField] protected GameObject explosion;
    private bool destroyed;

    protected virtual void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) Destroy(gameObject);
    }
    public virtual void destruction() {
        if (destroyed) return;
        destroyed = true;
        Destroy(gameObject);
        //instantiate explosion
    }

    protected bool defaultCollisionChecks(Collision collision) {
        if (collision.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d)) // hit a player
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableScript : MonoBehaviour
{
    private DamageFlash damageFlash;
    public GameObject Body;
    public float breakTime;
    public int hits;

    // Start is called before the first frame update
    void Start()
    {
        damageFlash = new DamageFlash(this.gameObject);
        damageFlash._flashTime = breakTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void destroy()
    {
        Destroy(gameObject, breakTime);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
        if(projectile != null)
        {
            
            GameObject parent = projectile.parent;
            if(parent == null)
            {
                return;
            }
            if(parent.tag == "Player")
            { 
                damageFlash.CallDamageFlash(this);
                hits--;
                if (hits <= 0)
                {
                    destroy();
                    projectile.destruction();
                }
            }
            else if (parent.tag == "Tank")
            {
                damageFlash.CallDamageFlash(this);
                hits--;
                if (hits == 0)
                {
                    destroy();
                }
            }
        }
    }
}

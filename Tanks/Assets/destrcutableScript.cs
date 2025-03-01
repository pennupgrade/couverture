using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destrcutableScript : MonoBehaviour
{
    private DamageFlash damageFlash;
    public GameObject Body;
    public int hits;

    // Start is called before the first frame update
    void Start()
    {
        damageFlash = new DamageFlash(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void destroy()
    {
        Destroy(this.gameObject, 0.25f);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
        if(projectile != null)
        {
            
            GameObject parent = projectile.parent;
            if(parent.tag == "Player")
            { 
                damageFlash.CallDamageFlash(this);
                hits--;
                if (hits == 0)
                {
                    destroy();
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

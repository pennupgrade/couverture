using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destrcutableScript : MonoBehaviour
{
    public Material red;
    public Material green;
    private DamageFlash damageFlash;
    public GameObject Body; // IK i told you that you can just put this.gameObject into DamageFlash, but decide if you're gonna keep this cause rn it's unused - Anthony
    // maybe add a counter in inspector to denote # of bullets the obj can take before it breaks - Anthony

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
        Bullet_Default bullet = collision.gameObject.GetComponent<Bullet_Default>();

        if(bullet != null)
        {
            // what if parent don't exist, or like say parent is an enemy tank but you kill that tank - Anthony
            // just make sure you cover that if you still want the below functionalities in the if branches
            GameObject parent = bullet.parent; 
            if(parent.tag == "Player")
            {
                Debug.Log("here");
                damageFlash.CallDamageFlash(this);
                destroy();
            }
            else if (parent.tag == "Tank")
            {
                Debug.Log("red");
                this.GetComponent<Renderer>().material = red;
            }
        }
    }
}

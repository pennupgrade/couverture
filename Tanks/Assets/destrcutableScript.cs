using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destrcutableScript : MonoBehaviour
{
    public Material red;
    public Material green;
    private DamageFlash damageFlash;
    public GameObject Body;

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

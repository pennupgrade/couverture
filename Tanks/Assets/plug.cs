using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plug : MonoBehaviour, IDestroyable
{
    // Start is called before the first frame update
    [SerializeField] private int HP; 
    [SerializeField] private Boss b;
    protected DamageFlash df;
    void Start()
    {
        df = new DamageFlash();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnColliderEnter() {
        
    }
    
    public void takeDamage(int dmg) {
        HP -= dmg;
        df.CallDamageFlash(this);
        if (HP <= 0) {
            b.unplug();
        }
    }

    
    public void incapacitate(float f) {

    }
}



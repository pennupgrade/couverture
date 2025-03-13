using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAttackScript : MonoBehaviour
{
    MissileScript missileScript;
    // Start is called before the first frame update
    void Start()
    {
        missileScript = transform.parent.GetComponent<MissileScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("damage zone collided with " + collider.tag);
        missileScript.handleAttack(collider);
    }
}

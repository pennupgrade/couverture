using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileColliderScript : MonoBehaviour
{
    MissileScript missileScript;
    // Start is called before the first frame update
    void Start()
    {
        missileScript = transform.parent.GetComponent<MissileScript>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collided with");
        Debug.Log(collider.tag);
        if (collider.tag == "MissileTargetZone")
        {
            missileScript.setHitTarget();
        }
    }
}

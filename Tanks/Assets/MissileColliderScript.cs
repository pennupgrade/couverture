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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
            missileScript.setHitTarget(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanColliderScript : MonoBehaviour
{
    FanScript fanScript;
    
    // Start is called before the first frame update
    void Start()
    {
        fanScript = transform.parent.GetComponent<FanScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "Player")
        {
            fanScript.pushPlayer(collider);
        }
    }
}

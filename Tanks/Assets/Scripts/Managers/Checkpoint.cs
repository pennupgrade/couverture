using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider c) {
        Tank t = c.gameObject.GetComponent<Tank>();
        if (t is not null) {
            CheckpointManager.CheckpointActivated(this);
        }
        print("CHECKPOINT ENTERED");

    }
}

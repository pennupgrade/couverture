using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider c) {
        Tank t = c.gameObject.GetComponent<Tank>();
        if (t is not null) {
            CheckpointManagerV2.CheckpointActivated(this);
        }
    }
}

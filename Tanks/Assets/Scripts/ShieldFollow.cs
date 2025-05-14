using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform follow;
    public ParticleSystem ps;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = follow.position + new Vector3(0, 0.3f, 0);
    }
}

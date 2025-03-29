using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlastMark : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bm;
    public LayerMask lm;
    void Start()
    {
        this.GetComponent<Enemy>().onDeath += SpawnBlast;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnBlast() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, lm))
        { 
            Instantiate(bm, this.transform.position,  Quaternion.FromToRotation(Vector3.up, hit.normal));
        }
    }
}

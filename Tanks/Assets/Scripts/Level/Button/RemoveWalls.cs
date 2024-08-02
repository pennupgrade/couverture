using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWalls : MonoBehaviour
{
    public GameObject[] toChange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent != null)
        if(other.transform.parent.tag == "Player")
        {
            foreach(GameObject g in toChange){
                g.gameObject.SetActive(!g.activeSelf);
            }
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }

}

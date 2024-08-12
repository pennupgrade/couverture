using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWalls : MonoBehaviour
{
    public bool reusable;
    private bool onCooldown;
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
        if (!onCooldown && other.transform.tag == "Player")
        {
            foreach (GameObject g in toChange) {
                if (g.TryGetComponent<Activatable>(out Activatable aObj)) {
                    aObj.activate();
                } else {
                    //for spawning enemies
                    g.gameObject.SetActive(!g.activeSelf);
                }
            }

            if (!reusable) {
                //for now
                gameObject.SetActive(!gameObject.activeSelf);
                //or 
                //onCooldown = true;
            } else {
                StartCoroutine(cooldownTimer());
            }
        }
    }
    private IEnumerator cooldownTimer() {
        onCooldown = true;
        yield return new WaitForSeconds(0.6f);
        onCooldown = false;
    }

}

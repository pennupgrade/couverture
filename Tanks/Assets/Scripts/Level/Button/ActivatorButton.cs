using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWalls : MonoBehaviour
{
    [SerializeField] private float yOffset = 2;
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
                StartCoroutine(slideButtonDown());
            } else {
                StartCoroutine(cooldownTimer());
            }
        }
    }
    private IEnumerator slideButtonDown() {
        onCooldown = true;
        float timer = 0;
        while (timer <= 1) {
            transform.position = Vector3.Lerp(transform.position, 
                transform.position + new Vector3(0, -yOffset, 0), timer);
            timer += Time.deltaTime * 0.5f;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    private IEnumerator cooldownTimer() {
        onCooldown = true;
        yield return new WaitForSeconds(0.6f);
        onCooldown = false;
    }

}

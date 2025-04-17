using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireDeath : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject explosionPrefab;
    public float breakForce;
    public bool breakable;
    private float startTime;
    void Start()
    {
        startTime = Time.time;;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + 2f && breakable) {
            this.GetComponent<CharacterJoint>().breakForce = breakForce;
        }
    }

    public void Kill() {
        var expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(expl, 2);
        this.gameObject.SetActive(false);
    }
}

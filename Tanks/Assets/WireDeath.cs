using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireDeath : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject explosionPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Kill() {
        var expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(expl, 2);
        this.gameObject.SetActive(false);
    }
}

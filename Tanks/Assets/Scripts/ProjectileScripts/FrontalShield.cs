using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontalShield : MonoBehaviour
{
    Material shieldMat;
    // Start is called before the first frame update
    void Start()
    {
        shieldMat = GetComponent<MeshRenderer>().material;
        StartCoroutine(startShield());
    }

    private IEnumerator startShield() {
        float i = 1;
        while (i > 0) {
            i -= 0.05f;
            shieldMat.SetFloat("_Dissolve", i);
            yield return new WaitForSeconds(0.06f);
        }
        shieldMat.SetFloat("_Dissolve", 0);
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("arrived");
        if (Vector3.Dot(other.transform.forward, transform.forward) > 0) return;
        Debug.Log("here");
        other.transform.forward = Vector3.Reflect(other.transform.forward, transform.forward);
        other.gameObject.GetComponent<Rigidbody>().velocity = other.transform.forward * other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
    }
}

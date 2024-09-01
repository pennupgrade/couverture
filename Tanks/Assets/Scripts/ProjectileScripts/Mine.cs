using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private bool isDead;
    public float lifetime;
    void Update() {
        lifetime -= Time.deltaTime;
        if (lifetime < 0) {
            destruction();
        }
    }
    void OnTriggerEnter(Collider collider) {
        if (isDead) return;
        if (collider.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d))
        {
            d.incapacitate(1);
            destruction();
        }
    }
    private void destruction() {
        if (isDead) return;
        isDead = true;
        GetComponent<SphereCollider>().enabled = false;

        Destroy(gameObject, 3);
    }
}

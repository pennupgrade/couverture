using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableClassic : MonoBehaviour, IDestroyable
{
    private DamageFlash damageFlash;
    [SerializeField] private float explosionForce;
    public GameObject originalObj;
    public GameObject fracturedObj;
    public int hits;
    private bool isDead;

    private GameObject fractObj;

    // Start is called before the first frame update
    void Start()
    {
        damageFlash = new DamageFlash(originalObj);
        damageFlash._flashTime = 0.2f;
    }

    public void incapacitate(float t) {}

    void fracture()
    {
        originalObj.SetActive(false);
        GetComponent<Collider>().enabled = false;
        if (fracturedObj == null) return;
        fracturedObj.SetActive(true);
        foreach(Transform t in fracturedObj.transform) {
            var rb = t.GetComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce, originalObj.transform.position, 2);
            StartCoroutine(Shrink(t, 1.3f));
        }
        Destroy(fracturedObj, 4);
    }

    private IEnumerator Shrink(Transform t, float delay) {
        yield return new WaitForSeconds(delay);
        t.gameObject.GetComponent<Collider>().enabled = false;
        Vector3 newScale = t.localScale;
        while (newScale.x > 0) {
            newScale -= new Vector3(10, 10, 10);
            if (t == null) break;
            t.localScale = newScale;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void takeDamage(int dmg) {
        hits -= dmg / 100;

        if (hits <= 0 && !isDead) {
            isDead = true;
            fracture();
            return;
        } else {
            damageFlash.CallDamageFlash(this);
        }
    }
    
}

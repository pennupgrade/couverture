using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableClassic : MonoBehaviour, IDestroyable
{
    private DamageFlash damageFlash;
    public GameObject originalObj;
    public GameObject fracturedObjPrefab;
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
        if (fracturedObjPrefab == null) return;
        fractObj = Instantiate(fracturedObjPrefab) as GameObject;
        foreach(Transform t in fractObj.transform) {
            var rb = t.GetComponent<Rigidbody>();
            rb.AddExplosionForce(0.5f, originalObj.transform.position, 1);
            StartCoroutine(Shrink(t, 2));
        }
        Destroy(fractObj, 4);
    }

    private IEnumerator Shrink(Transform t, float delay) {
        yield return new WaitForSeconds(delay);
        Vector3 newScale = t.localScale;
        while (newScale.x > 0) {
            newScale -= new Vector3(1, 1, 1);
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

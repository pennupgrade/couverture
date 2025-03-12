using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public Material material;
    [SerializeField] private float changeRate;
    // Start is called before the first frame update

    void Awake() {
        material = GetComponent<MeshRenderer>().material;
    }

    public void StartBulletTrail() {
        material.SetFloat("_Alpha", 1);
        StartCoroutine(AnimateMaterial());
    }

    private IEnumerator AnimateMaterial() {
        float val = material.GetFloat("_Alpha");
        while (val > 0) {
            val -= changeRate;
            material.SetFloat("_Alpha", val);
            yield return new WaitForSeconds(0.1f);
        }
        PoolManager.bulletTrailPool.Release(this);
    }
}

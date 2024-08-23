using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public GameObject meshPrefab;
    public float destroyTime;
    public float changeRate;
    public float spawnRate;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnMesh", 0.2f, spawnRate);
    }

    void SpawnMesh() {
        GameObject gm = Instantiate(meshPrefab);
        gm.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        MeshRenderer mr = gm.GetComponent<MeshRenderer>();
        StartCoroutine(AnimateMaterial(mr.material, changeRate));
        Destroy(gm, destroyTime);
    }
    private IEnumerator AnimateMaterial(Material mat, float rate) {
        float val = mat.GetFloat("_Alpha");
        while (val > 0) {
            val -= rate;
            mat.SetFloat("_Alpha", val);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void kill() {
        CancelInvoke();
        Destroy(gameObject, 1.5f);
    }
}

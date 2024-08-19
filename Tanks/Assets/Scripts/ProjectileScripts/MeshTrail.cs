using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public Material mat;
    public Mesh mesh;
    public Vector3 scale;
    public float destroyTime;
    public float changeRate;
    public float spawnRate;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnMesh", 0, spawnRate);
    }

    void SpawnMesh() {
        GameObject gm = new GameObject();
        gm.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        gm.transform.localScale = scale;
        MeshFilter mf = gm.AddComponent<MeshFilter>();
        MeshRenderer mr = gm.AddComponent<MeshRenderer>();
        mr.material = mat;
        mf.mesh = mesh;
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

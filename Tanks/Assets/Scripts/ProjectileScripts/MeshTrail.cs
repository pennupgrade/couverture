using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public GameObject meshPrefab;
    public float destroyTime;
    public float spawnRate;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    public void StartTrail() {
        InvokeRepeating("SpawnMesh", 0.2f, spawnRate);
    }

    void SpawnMesh() {
        BulletTrail gm = PoolManager.bulletTrailPool.Get();
        gm.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
    }
    
    
    public void kill() {
        CancelInvoke();
        // Destroy(gameObject, 1.5f);
    }
}

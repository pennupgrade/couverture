using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager
{
    private static readonly int maxSizeTrailPool = 50;
    private static readonly int defaultSizeTrailPool = 10;

    private static GameObject bulletTrailPrefab = Resources.Load<GameObject>("BulletTrail");
    public static IObjectPool<BulletTrail> bulletTrailPool = new ObjectPool<BulletTrail>(CreateTrail, ActionOnGetTrail, ActionOnReleaseTrail, ActionOnDestroyTrail, true, defaultSizeTrailPool, maxSizeTrailPool);


    private static BulletTrail CreateTrail() {
        GameObject trail = Object.Instantiate(bulletTrailPrefab);
        Object.DontDestroyOnLoad(trail);
        return trail.GetComponent<BulletTrail>();
    }

    private static void ActionOnGetTrail(BulletTrail trail) {
        trail.gameObject.SetActive(true);
        trail.StartBulletTrail();
    }

    private static void ActionOnReleaseTrail(BulletTrail trail) {
        trail.gameObject.SetActive(false);
    }
    private static void ActionOnDestroyTrail(BulletTrail trail) {
        Object.Destroy(trail.gameObject);
    }
}

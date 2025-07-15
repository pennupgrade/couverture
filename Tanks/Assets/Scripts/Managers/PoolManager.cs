// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class PoolManager
{
    private static readonly int maxSizeTrailPool = 50;
    private static readonly int defaultSizeTrailPool = 50;

    private static readonly int maxSizeBulletPool = 20;
    private static readonly int defaultSizeBulletPool = 20;

    private static GameObject bulletTrailPrefab = Resources.Load<GameObject>("BulletTrail");
    private static GameObject bulletPrefab = Resources.Load<GameObject>("Bullet");
    public static IObjectPool<BulletTrail> bulletTrailPool = new ObjectPool<BulletTrail>(CreateTrail, ActionOnGetTrail, ActionOnReleaseTrail, ActionOnDestroyTrail, true, defaultSizeTrailPool, maxSizeTrailPool);

    public static IObjectPool<Bullet_Default> bulletPool = new ObjectPool<Bullet_Default>(CreateBullet, ActionOnGetBullet, ActionOnReleaseBullet, ActionOnDestroyBullet, true, defaultSizeBulletPool, maxSizeBulletPool);

    private static Bullet_Default CreateBullet() {
        GameObject bullet = Object.Instantiate(bulletPrefab);
        Object.DontDestroyOnLoad(bullet);
        return bullet.GetComponent<Bullet_Default>();
    }

    private static void ActionOnGetBullet(Bullet_Default bullet) {
        bullet.gameObject.SetActive(true);
        SceneManager.sceneLoaded += bullet.OnSceneLoaded;
        bullet.StartBullet();
    }

    private static void ActionOnReleaseBullet(Bullet_Default bullet) {
        SceneManager.sceneLoaded -= bullet.OnSceneLoaded;
        bullet.meshTrail.kill();
        bullet.gameObject.SetActive(false);
    }

    private static void ActionOnDestroyBullet(Bullet_Default bullet) {
        SceneManager.sceneLoaded -= bullet.OnSceneLoaded;
        if (bullet is null) {
            Debug.Log("NULL ERROR HERE");
        }
        bullet.meshTrail.kill();
        Object.Destroy(bullet.gameObject);
    }


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

// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    private Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.gameObject.GetComponent<Enemy>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Projectile" && other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            enemy.bulletWarn(rb);
        }
    }
}

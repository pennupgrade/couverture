// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEScript : MonoBehaviour
{
    private float radius;
    [SerializeField]
    private float maxBlastDamage; // say 100

    // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Physics.OverlapSphere.html
    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.TryGetComponent<IDestroyable>(out IDestroyable d)) // hit a player
            {
                Vector3 hitPos = hitCollider.transform.position;
                float power = Vector3.Distance(hitPos, center) / radius;

                d.takeDamage( (int) (maxBlastDamage * ( 1.1f - power )) );
            }
        }

        Destroy(this, 0.1f);
    }
    
    void Start()
    {
        radius = transform.localScale.x;
        ExplosionDamage(transform.position, radius);
    }
}

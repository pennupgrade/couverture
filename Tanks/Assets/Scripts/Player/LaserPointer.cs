// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer2 : MonoBehaviour
{
    LineRenderer lineRenderer;

    public float maxDistance;

    public LayerMask layersToIgnore;

    public Transform endTransform;


    // Update is called once per frame
    void Update()
    {
        setPos();
    }
    public void setPos() {
        if (lineRenderer == null) {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDistance, ~layersToIgnore))
        {
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * maxDistance);
        }

        endTransform.position = lineRenderer.GetPosition(1);
    }
}

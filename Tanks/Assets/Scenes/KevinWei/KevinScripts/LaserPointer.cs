using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    LineRenderer lineRenderer;

    public float maxDistance;

    public LayerMask layersToIgnore;

    public Transform endTransform;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusionFade : MonoBehaviour
{
    public GameObject playerObj;
    public LayerMask obstacleLayer;

    void Update()
    {
        Ray ray = new(transform.position, Vector3.Normalize(playerObj.transform.position - transform.position));
        Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(playerObj.transform.position, transform.position), obstacleLayer);

        // Debug.Log("Camera Hit: " + (hit.collider ? hit.collider.gameObject : "None"));
    }
}

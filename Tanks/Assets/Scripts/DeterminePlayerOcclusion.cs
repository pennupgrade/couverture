using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeterminePlayerOcclusion : MonoBehaviour
{
    Tank player;
    bool isVisible = false;

    MeshRenderer meshRenderer;
    Material material;

    void Start()
    {
        player = FindObjectOfType<Tank>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null) return;

        material = meshRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        // Send raycasts
        if (player && material)
        {
            material.SetVector("_PlayerPos", player.transform.position);
            material.SetVector("_Camera", Camera.main.transform.position);
        }
    }
}

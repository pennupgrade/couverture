// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using UnityEngine;

public class DeterminePlayerOcclusion : MonoBehaviour
{
    private Tank player;

    private MeshRenderer meshRenderer;
    private Material material;

    private void Start() {
        player = FindObjectOfType<Tank>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null) return;

        material = meshRenderer.material;
    }

    // Update is called once per frame
    private void Update() {
        // Send raycasts
        if (player && material) {
            material.SetVector("_PlayerPos", player.transform.position);
            material.SetVector("_Camera", Camera.main.transform.position);
        }
    }
}
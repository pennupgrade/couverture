// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainToMesh))]
public class TerrainToMeshEditor : Editor
{
    TerrainToMesh terrainToMesh;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Rebake Terrain"))
        {
            terrainToMesh.Generate();
        }

    }

    private void OnEnable()
    {
        terrainToMesh = (TerrainToMesh)target;

        terrainToMesh.Generate();
    }
}
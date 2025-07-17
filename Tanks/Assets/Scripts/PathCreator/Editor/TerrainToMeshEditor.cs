// SPDX-FileCopyrightText: 2019 Sebastian Lague
// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: LicenseRef-MIT-PathCreator
// SPDX-License-Identifier: MPL-2.0

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
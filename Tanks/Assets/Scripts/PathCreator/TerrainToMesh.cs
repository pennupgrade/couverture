// SPDX-FileCopyrightText: 2019 Sebastian Lague
// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: LicenseRef-MIT-PathCreator
// SPDX-License-Identifier: MPL-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainToMesh : MonoBehaviour
{
    public Terrain terrain;

    public Mesh mesh;

    public void Generate()
    {
        if (terrain == null) terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            mesh = GenerateMeshFromTerrain(terrain.terrainData);
            // Use terrainMesh as needed, e.g., assign to a MeshFilter.
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    private Mesh GenerateMeshFromTerrain(TerrainData terrainData)
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uv = new Vector2[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        int t = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                float heightValue = heights[y, x];
                //vertices[index] = new Vector3(x, heightValue * terrainData.size.y, y);

                vertices[index] = new Vector3(
                    x * terrainData.size.x / (width - 1),
                    heightValue * terrainData.size.y,
                    y * terrainData.size.z / (height - 1)
                );

                uv[index] = new Vector2((float)x / width, (float)y / height);

                if (x < width - 1 && y < height - 1)
                {
                    int bottomLeft = index;
                    int bottomRight = index + 1;
                    int topLeft = index + width;
                    int topRight = index + width + 1;

                    // First triangle
                    triangles[t++] = bottomLeft;
                    triangles[t++] = topLeft;
                    triangles[t++] = topRight;

                    // Second triangle
                    triangles[t++] = bottomLeft;
                    triangles[t++] = topRight;
                    triangles[t++] = bottomRight;
                }
            }
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uv
        };

        mesh.RecalculateNormals();
        return mesh;
    }


}

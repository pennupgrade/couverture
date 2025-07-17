// SPDX-FileCopyrightText: 2024 The Catanks Contributors
//
// SPDX-License-Identifier: MPL-2.0

using PathCreation;
using UnityEngine;

public class MoveAlongPath2 : MonoBehaviour
{
    public VertexPath vertexPath;

    public float speed = 4f;

    float time = 0f;

    public bool lockRotation;

    // Update is called once per frame
    void Update()
    {
        transform.position = vertexPath.MoveConstantVelocity(speed, advanceForward: true, ref time);

        if (!lockRotation)
        {
            transform.forward = vertexPath.GetTangent(time);
        }
    }

}

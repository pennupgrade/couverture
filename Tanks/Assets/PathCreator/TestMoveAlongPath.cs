using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveAlongPath : MonoBehaviour
{
    public VertexPath vertexPath;

    public float speed = 4f;

    float time = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.position = vertexPath.MoveConstantVelocity(speed, advanceForward: true, ref time);

        transform.forward = vertexPath.GetTangent(time);
    }

}

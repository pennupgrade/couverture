using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformLinear : MovingPlatform
{
    public VertexPath vertexPath;
    public float speed = 4f;

    public List<Transform> passengerList = new();
    float time = 0f;

    void Start()
    {
        vertexPath.GenerateTangentList(1);
        vertexPath.DebugRenderTracks();
    }

    // Update is called once per frame
    void Update() 
    {
        Vector3 oldPos = transform.position;
        transform.position = vertexPath.MoveConstantVelocity(speed, advanceForward: true, ref time);

        //transform.forward = vertexPath.GetTangent(time);
        for (int i = 0; i < passengerList.Count; i++)
        {
            if (passengerList[i] == null)
            {
                passengerList.RemoveAt(i);
                i -= 1;
                continue;
            }
            passengerList[i].position += transform.position - oldPos;
        }

        nextIndex = vertexPath.GetNextIndex((int) time);
    }

    void OnTriggerEnter(Collider other)
    {
        if (passengerList.Contains(other.transform))
        {
            return;
        }
        if (other.gameObject.TryGetComponent(out Tank t)) 
        {
            passengerList.Add(t.transform);
            t.SetPlatformSpeed(speed);
        }
        if (other.gameObject.TryGetComponent(out Enemy e)) 
        {
            passengerList.Add(e.transform);
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (!passengerList.Contains(other.transform))
        {
            return;
        }

        if (other.gameObject.TryGetComponent(out Tank t))
        {
            t.SetPlatformSpeed(-speed);
        }

        passengerList.Remove(other.transform);
    }
}

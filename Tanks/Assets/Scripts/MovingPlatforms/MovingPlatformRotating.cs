using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformRotating : MonoBehaviour
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
        Vector3 oldRot = transform.rotation.eulerAngles;
        transform.position = vertexPath.MoveConstantVelocity(speed, advanceForward: true, ref time);

        transform.forward = vertexPath.GetTangent(time);
        for (int i = 0; i < passengerList.Count; i++)
        {
            if (passengerList[i] == null)
            {
                passengerList.RemoveAt(i);
                i -= 1;
                continue;
            }
            passengerList[i].position += transform.position - oldPos;
            passengerList[i].RotateAround(transform.position, Vector3.up, transform.rotation.eulerAngles.y - oldRot.y);
        }
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
        passengerList.Remove(other.transform);
    }
}

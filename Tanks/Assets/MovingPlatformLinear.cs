using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformLinear : MonoBehaviour
{
    public VertexPath vertexPath;
    public float speed = 4f;

    public List<Transform> passengerList = new();

    // Update is called once per frame
    void Update() 
    {
        Vector3 oldPos = transform.position;
        Vector3 oldRot = transform.rotation.eulerAngles;

        transform.position = vertexPath.MoveConstantVelocity(speed, advanceForward: true);
        transform.forward = vertexPath.GetTangent();

        Vector3 deltaPos = transform.position - oldPos;
        float deltaAngY = transform.rotation.eulerAngles.y - oldRot.y;

        for (int i = 0; i < passengerList.Count; i++)
        {
            if (passengerList[i] == null)
            {
                passengerList.RemoveAt(i);
                i -= 1;
                continue;
            }

            passengerList[i].position += deltaPos;
            // passengerList[i].RotateAround(transform.position, Vector3.up, transform.rotation.eulerAngles.y - oldRot.y);
            passengerList[i].RotateAround(transform.position, Vector3.up, deltaAngY);
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

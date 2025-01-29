
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveAlongPath : MonoBehaviour
{
    public VertexPath vertexPath;
    public float speed = 4f;

    public List<Transform> passengerList = new();

    float time = 0f;

    // Update is called once per frame
    void Update() 
    {
        Vector3 oldPos = transform.position;
        transform.position = vertexPath.MoveConstantVelocity(speed, advanceForward: true);
        for (int i = 0; i < passengerList.Count; i++)
        {
            passengerList[i].position += transform.position - oldPos;
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

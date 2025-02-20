using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBackAndForth : MonoBehaviour
{
    public VertexPath vertexPath;
    public float period = 4f;

    public List<Transform> passengerList = new();
    public float equilibriumVelocity = 1.3f;

    void Start()
    {
        vertexPath.GenerateTangentList(1);
        vertexPath.DebugRenderTracks();
    }

    // Update is called once per frame
    void Update() 
    {
        Vector3 oldPos = transform.position;
        transform.position = param(Time.time) * vertexPath.GetMainPos(0) + (1-param(Time.time)) * vertexPath.GetMainPos(1);

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
    }

    private float param(float t)
    {
        return Mathf.Clamp(equilibriumVelocity * Mathf.Sin(Mathf.PI * t / period)*Mathf.Sin(Mathf.PI * t / period) - (equilibriumVelocity - 1) / 2, 0f, 1f);
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

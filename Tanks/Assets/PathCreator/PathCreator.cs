using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PathCreation
{
    [RequireComponent(typeof(VertexPath))]
    public class PathCreator : MonoBehaviour
    {
        [HideInInspector()]
        public bool isLooping;
        [HideInInspector()]
        public bool isLinear;

        public VertexPath path;

        public MeshFilter meshFilter;

        // Draws the sphere gizmos
        public void OnDrawGizmosSelected()
        {
            if (path != null)
            {
                for (int i = 0; i < path.VertexCount(); i++)
                {
                    Gizmos.color = PathCreationSettings.sphereColor;
                    Gizmos.DrawSphere(path.GetMainPos(i), PathCreationSettings.mainPointGizmoRadius);
                }
            }
            else
            {
                path = GetComponent<VertexPath>();
            }
        }

        public void AddVertex()
        {
            Vector3 newLocation;
            if (path.VertexCount() > 0) 
            {
                newLocation = path.GetMainPos(path.VertexCount() - 1) + new Vector3(0, 0, 1);
            }
            else
            {
                newLocation = path.transform.position + new Vector3(0, 0, 1);
            }

            AddVertex(newLocation);
        }

        public void AddVertex(Vector3 pos)
        {
            path.AddVertex(pos);
        }

        public void RemoveVertex(int i)
        {
            path.RemoveVertex(i);
        }
    }
}


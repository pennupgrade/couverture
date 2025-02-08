using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace PathCreation
{
    [CustomEditor(typeof(VertexPath))]
    public class VertexPathEditor : Editor
    {
        VertexPath vertexPath;
        Transform transform;

        private void OnEnable()
        {
            vertexPath = (VertexPath)target;

            transform = vertexPath.transform;

            // Initialize lastPos when the editor script is enabled
            lastPos = transform.position;
            lastRot = transform.rotation;
            lastScale = transform.localScale;
        }

        Vector3 lastPos;
        Quaternion lastRot;
        Vector3 lastScale;

        public void OnSceneGUI()
        {
            ManagePosition();
            ManageRotation();
            ManageScale();
        }

        private void ManagePosition()
        {
            Undo.ClearUndo(this);

            if (lastPos == null) lastPos = transform.position;
            Vector3 delta = transform.position - lastPos;

            if (transform.position != lastPos)
            {
                for (int i = 0; i < vertexPath.VertexCount(); i++)
                {
                    vertexPath.SetMainPos(i, vertexPath.GetMainPos(i) + delta, false);
                }
            }

            lastPos = transform.position;
        }

        private void ManageRotation()
        {
            Undo.ClearUndo(this);

            if (lastRot == null) lastRot = transform.rotation;
            Quaternion relativeRotation = Quaternion.Inverse(lastRot) * transform.rotation;

            if (transform.rotation != lastRot)
            {
                for (int i = 0; i < vertexPath.VertexCount(); i++)
                {
                    Vector3 p = vertexPath.GetMainPos(i);

                    Vector3 dir = p - transform.position; // get point direction relative to pivot
                    dir = relativeRotation * dir; // rotate it

                    vertexPath.SetMainPos(i, dir + transform.position, false);
                    vertexPath.RotateVertex(i, relativeRotation, false);
                }
            }

            lastRot = transform.rotation;
        }

        //NewPosition=Pivot+(OriginalPosition-Pivot)*ScaleFactor
        private void ManageScale()
        {
            Undo.ClearUndo(this);

            if (transform.localScale.sqrMagnitude == 0f)
            {
                transform.localScale.Normalize();
            }

            if (lastScale == null) lastScale = transform.localScale;
            Vector3 deltaScale = (transform.localScale - lastScale);

            deltaScale = new Vector3(deltaScale.x / lastScale.x, deltaScale.y / lastScale.y, deltaScale.z / lastScale.z);

            if (transform.localScale != lastScale)
            {
                for (int i = 0; i < vertexPath.VertexCount(); i++)
                {
                    // scale it out
                    Vector3 pivot = transform.position;

                    Vector3 vert = vertexPath.GetMainPos(i);
                    Vector3 dir = (vert - pivot);

                    Vector3 scaler = Vector3.one + deltaScale;

                    Vector3 displace = new Vector3(dir.x * scaler.x, dir.y * scaler.y, dir.z * scaler.z);

                    Vector3 newPos = pivot + displace;

                    vertexPath.SetMainPos(i, newPos, false);
                    vertexPath.ScaleVertex(i, deltaScale, false);
                }
            }

            lastScale = transform.localScale;
        }

    }

}

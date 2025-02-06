using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace PathCreation
{
    [System.Serializable]
    public class VertexPath : MonoBehaviour
    {
        [SerializeField]
        private List<Vertex> vertices;

        [HideInInspector()]
        public bool isLinear;
        [HideInInspector()]
        public bool isLooping;

        public void Reset()
        {
            Undo.RecordObject(this, "Reset Path");
            vertices.Clear();
        }

        #region Getters Setters
        public void AddVertex(Vector3 v)
        {
            Undo.RecordObject(this, "Added Vertex");
            vertices.Add(new Vertex(v));
        }

        public void RemoveVertex(int i)
        {
            Undo.RecordObject(this, "Removed Vertex");
            vertices.Remove(vertices[i]);
        }

        public int VertexCount()
        {
            return vertices.Count;
        }

        public Vector3 GetMainPos(int index)
        {
            if (index < 0 || index >= vertices.Count) return vertices[index%VertexCount()].mainPos;
            return vertices[index].mainPos;
        }

        public void SetMainPos(int index, Vector3 pos, bool record)
        {
            if (record)
                Undo.RecordObject(this, "Moved Main Vertex");

            Vector3 change = pos - vertices[index].mainPos;
            vertices[index].controlPoint1 += change;
            vertices[index].controlPoint2 += change;

            //Vector3 toThisTransform = pos - this.transform.position;

            vertices[index].mainPos = pos;
        }

        public void SetMainPos(int index, Vector3 pos)
        {
            SetMainPos(index, pos, true);
        }

        public void SetControlPos(int index, int control, Vector3 pos, int modeInt, bool record)
        {
            if (record)
                Undo.RecordObject(this, "Moved Control Vertex");

            Vector3 center = GetMainPos(index);
            Vector3 vectorToCenter = pos - center;

            Vector3 otherTarget = control == 1 ? vertices[index].controlPoint2 : vertices[index].controlPoint1;
            
            switch (modeInt)
            {
                // Aligned
                case 0:

                    float dist = (otherTarget - center).magnitude;

                    if (control == 1)
                    {
                        vertices[index].controlPoint1 = pos;
                        vertices[index].controlPoint2 = center - vectorToCenter.normalized * dist;
                    }
                    else if (control == 2)
                    {
                        vertices[index].controlPoint2 = pos;
                        vertices[index].controlPoint1 = center - vectorToCenter.normalized * dist;
                    }
                    break;

                // Mirrored
                case 1:
                    if (control == 1)
                    {
                        vertices[index].controlPoint1 = pos;
                        vertices[index].controlPoint2 = center - vectorToCenter;
                    }
                    else if (control == 2)
                    {
                        vertices[index].controlPoint2 = pos;
                        vertices[index].controlPoint1 = center - vectorToCenter;
                    }
                    break;

                // Free
                case 2:
                    if (control == 1)
                    {
                        vertices[index].controlPoint1 = pos;
                    }
                    else if (control == 2)
                    {
                        vertices[index].controlPoint2 = pos;
                    }
                    break;

                default:
                    break;
            }
        }

        public void SetControlPos(int index, int control, Vector3 pos, int modeInt)
        {
            SetControlPos(index, control, pos, modeInt, true);
        }


        public Vector3 GetControlPos(int index, int control)
        { 
            return control == 1 ? vertices[index].controlPoint1 : vertices[index].controlPoint2;
        }

        public void RotateVertex(int i, Quaternion q, bool record)
        {
            if (record)
                Undo.RecordObject(this, "Rotated Vertex");

            vertices[i].rotation *= q;

            Vector3 pivot = GetMainPos(i);

            Vector3 dir1 = GetControlPos(i, 1) - pivot; // get point direction relative to pivot
            dir1 = q * dir1; // rotate it

            Vector3 dir2 = GetControlPos(i, 2) - pivot; // get point direction relative to pivot
            dir2 = q * dir2; // rotate it

            SetControlPos(i, 1, dir1 + pivot, 2, record);
            SetControlPos(i, 2, dir2 + pivot, 2, record);
        }

        public void ScaleVertex(int i,  Vector3 deltaScale, bool record)
        {
            if (record)
                Undo.RecordObject(this, "Scaled Vertex");

            // scale it out
            Vector3 pivot = GetMainPos(i);

            Vector3 vert1 = GetControlPos(i, 1);
            Vector3 vert2 = GetControlPos(i, 2);

            Vector3 dir1 = (vert1 - pivot);
            Vector3 dir2 = (vert2 - pivot);

            Vector3 scaler = Vector3.one + deltaScale;

            Vector3 displace1 = new Vector3(dir1.x * scaler.x, dir1.y * scaler.y, dir1.z * scaler.z);
            Vector3 displace2 = new Vector3(dir2.x * scaler.x, dir2.y * scaler.y, dir2.z * scaler.z);

            Vector3 newPos1 = pivot + displace1;
            Vector3 newPos2 = pivot + displace2;

            SetControlPos(i, 1, newPos1, 2, record);
            SetControlPos(i, 2, newPos2, 2, record);
        }

        public Vector3 GetVertexScale(int i)
        {
            return vertices[i].scale;
        }

        public bool GetIsBeingMoved(int i)
        {
            return vertices[i].isBeingMoved;
        }

        public bool OneIsMoving()
        {
            foreach (var b in vertices)
            {
                if (b.isBeingMoved)
                    return true;
            }
            return false;
        }

        public void SetIsBeingMoved(int i, bool b)
        {
            vertices[i].isBeingMoved = b;
        }

#endregion

#region Movement and Lerping and BLerping

        float currT = 0f;

        // get the next position based on the current index you are on
        public Vector3 Lerp(int startIndex, float t)
        {
            Vector3 startMainPos = GetMainPos(startIndex);

            int nextIndex = GetNextIndex(startIndex);

            if (nextIndex == -1)
            {
                return startMainPos;
            }

            return Vector3.Lerp(startMainPos, GetMainPos(nextIndex), t);
        }

        // get the next position based on positions
        public Vector3 Lerp(Vector3 p0, Vector3 p1, float t)
        {
            return Vector3.Lerp(p0, p1, t);
        }

        // bezier lerp
        public Vector3 BLerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            // Bernstein polynomial form
            Vector3 pt = 
                p0 * (-Mathf.Pow(t, 3) + 3 * Mathf.Pow(t,2) -3f * t + 1f) +
                p1 * (3 * Mathf.Pow(t, 3) - 6 * Mathf.Pow(t, 2) + 3 * t) + 
                p2 * (-3 * Mathf.Pow(t, 3) + 3 * Mathf.Pow(t, 2)) + 
                p3 * (Mathf.Pow(t, 3));

            return pt;
        }

        // this function is shit
        //public Vector3 MoveLerp(float t)
        //{
        //    // find start index based on t
        //    int startIndex = (int)t;

        //    if (isLooping)
        //    {
        //        startIndex %= VertexCount();
        //    }

        //    return Lerp(startIndex, , t - startIndex);
        //}

        public Vector3 MoveConstantVelocity(float vel, bool advanceForward)
        {
            // find start index based on t
            int startIndex = (int) currT;

            int nextIndex = GetNextIndex(startIndex);

            Vector3 p0 = GetMainPos(startIndex);

            if (nextIndex == -1)
            {
                return p0;
            }

            Vector3 p1 = GetControlPos(startIndex, 1);
            Vector3 p2 = GetControlPos(nextIndex, 2);
            Vector3 p3 = GetMainPos(nextIndex);

            float localT = (currT - startIndex);

            if (advanceForward)
            {
                float distance = isLinear ? Vector3.Distance(GetMainPos(startIndex), GetMainPos(nextIndex))
                    : GetBezierArcLength(p0, p1, p2, p3);

                currT += Time.deltaTime * vel / distance;

                if (isLooping && currT >= VertexCount())
                {
                    currT = 0f;
                }
            }


            if (isLinear)
            {
                return Lerp(p0, p3, localT);
            }
            else
            {
                return BLerp(p0, p1, p2, p3, localT);
            }
        }

        public Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 dpdt =
                p0 * (-3f * Mathf.Pow(t, 2) + 6f * t - 3f) +
                p1 * (9f * Mathf.Pow(t, 2) - 12f * t + 3f) +
                p2 * (-9f * Mathf.Pow(t, 2) + 6f * t) +
                p3 * (3f * Mathf.Pow(t, 2));

            return dpdt;
        }

        public Vector3 GetTangent()
        {
            // find start index based on t
            int startIndex = (int)currT;

            int nextIndex = GetNextIndex(startIndex);

            Vector3 p0 = GetMainPos(startIndex);

            if (nextIndex == -1)
            {
                return p0;
            }

            float localT = (currT - startIndex);

            Vector3 p1 = GetControlPos(startIndex, 1);
            Vector3 p2 = GetControlPos(nextIndex, 2);
            Vector3 p3 = GetMainPos(nextIndex);

            if (isLinear)
            {
                return p3 - p0;
            }

            return GetTangent(p0, p1, p2, p3, localT);
        }
        public float GetBezierArcLength(Vector3 p0,  Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return Vector3.Distance(p1, p2) + Vector3.Distance(p0, p1) + Vector3.Distance(p2, p3);
        }
            
        public int GetNextIndex(int currIndex)
        {
            int nextIndex = currIndex + 1;

            if (nextIndex >= VertexCount())
            {
                if (this.isLooping)
                {
                    nextIndex = 0;
                }
                else
                {
                    return -1;
                }
            }

            return nextIndex;
        }

#endregion

        //public float GetDistanceBetweenPoints(int index1, int index2)
        //{
        //    if (index1 == -1 || index2 == -1)
        //    {
        //        return 0f;
        //    }

        //    if (isLinear)
        //    {
        //        return Vector3.Distance(GetMainPos(index1), GetMainPos(index2));
        //    }
        //    else
        //    {
        //        return 1f;  // how do you calculate arc length??? :(((
        //    }
        //}

    }

    [System.Serializable]
    public class Vertex
    {
        public Vector3 mainPos;

        public Vector3 controlPoint1;
        public Vector3 controlPoint2;

        public bool isBeingMoved;

        public Quaternion rotation;

        public Vector3 scale;

        public Vertex(Vector3 position)
        {
            mainPos = position;

            controlPoint1 = position + Vector3.right;
            controlPoint2 = position - Vector3.right;

            scale = Vector3.one;
        }
    }

}

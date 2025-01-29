using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

namespace PathCreation
{
    // Custom editor for the character path
    [CustomEditor(typeof(PathCreator))]
    public class PathCreatorEditor : Editor
    {
        PathCreator pathCreator;
        VertexPath vertexPath;
        Camera cam;

        string[] toolbarStrings = { "No snapping", "Snap to mesh", "Snap flat" };

        string[] controlHandleModeStrings = { "Aligned", "Mirrored", "Free" };


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            pathCreator.isLinear = GUILayout.Toggle(pathCreator.isLinear, "Linear Mode");
            vertexPath.isLinear = pathCreator.isLinear;

            pathCreator.isLooping = GUILayout.Toggle(pathCreator.isLooping, "Is Looping");
            vertexPath.isLooping = pathCreator.isLooping;

            GUILayout.Space(20);

            if (GUILayout.Button("Add Vertex"))
            {
                AddVertex();
            }

            if (GUILayout.Button("Reset Path"))
            {
                vertexPath.ResetPath();
                SceneView.RepaintAll();
            }

            GUILayout.Space(20);

            GUILayout.Label("Snapping");
            ToolbarInt = GUILayout.Toolbar(ToolbarInt, toolbarStrings);

            if (!pathCreator.isLinear)
            {
                GUILayout.Label("Control Handle Mode");
                ControlHandleInt = GUILayout.Toolbar(ControlHandleInt, controlHandleModeStrings);
            }

            if (cam == null)
                cam = SceneView.lastActiveSceneView.camera;
        }


        private void OnEnable()
        {
            pathCreator = (PathCreator)target;
        
            if (pathCreator.path == null)
            {
                pathCreator.path = pathCreator.GetComponent<VertexPath>();
            }

            vertexPath = pathCreator.path;

            if (cam == null && SceneView.lastActiveSceneView.camera != null)
                cam = SceneView.lastActiveSceneView.camera;
        }

        private void OnDisable()
        {
            ToolbarInt = 0;
        }

        private void AddVertex()
        {
            pathCreator.AddVertex();
            SceneView.RepaintAll();
        }

        private void AddVertex(Vector3 pos)
        {
            pathCreator.AddVertex(pos);
            SceneView.RepaintAll();
        }

        private void RemoveVertex(int i)
        {
            pathCreator.RemoveVertex(i);
        }

        public void OnGUI()
        {
            if (pathCreator.path == null)
            {
                pathCreator.path = pathCreator.GetComponent<VertexPath>();
            }
        }

        // Adding vertices
        public void OnSceneGUI()
        {
            if (cam == null && SceneView.lastActiveSceneView.camera != null)
                cam = SceneView.lastActiveSceneView.camera;

            // add a new thing on shift click?
            if (Event.current.keyCode == KeyCode.A && Event.current.type == EventType.KeyDown)
            {
                if (Event.current.shift)
                {
                    if (vertexPath.VertexCount() == 0)
                    {
                        AddVertex();
                        return;
                    }

                    Vector3 mousePos = (
                        HandleUtility.GUIPointToScreenPixelCoordinate(Event.current.mousePosition));

                    Plane p = new Plane(cam.transform.forward, cam.transform.position);

                    Vector3 oldVector = GetLastVector();
                    mousePos.z = cam.nearClipPlane + p.GetDistanceToPoint(oldVector);

                    Vector3 worldMousePos = cam.ScreenToWorldPoint(mousePos);

                    AddVertex(worldMousePos);

                    Vector3 v = GetLastVector();

                    vertexPath.SetMainPos(vertexPath.VertexCount() - 1, ApplySnapping(v, oldVector));
                }
            }
            DrawPointMovementGizmos();
        }

        private void DrawPointMovementGizmos()
        {
            if (Event.current.keyCode == KeyCode.B)
            {
                ToolbarInt = 0;
            }
            else if (Event.current.keyCode == KeyCode.N)
            {
                ToolbarInt = 1;
            }
            else if (Event.current.keyCode == KeyCode.M)
            {
                ToolbarInt = 2;
            }

            // Draw free move handles
            for (int i = 0; i < vertexPath.VertexCount(); i++)
            {
                Vector3 currVertexPos = vertexPath.GetMainPos(i);

                DrawControlMovementGizmos(i);

                // deletion
                if (Event.current.shift)
                {
                    if (Handles.Button(currVertexPos, Quaternion.identity, PathCreationSettings.mainPointGizmoRadius, 0.1f, Handles.CubeHandleCap))
                    {
                        RemoveVertex(i);
                    }
                }
                else
                {
                    vertexPath.SetMainPos(i, DefineMovementGizmos(currVertexPos, PathCreationSettings.mainPointGizmoRadius, PathCreationSettings.defaultColor, i));
                }
            }
        }

        private void DrawControlMovementGizmos(int vertexIndex)
        {
            int nextIndex = vertexPath.GetNextIndex(vertexIndex);

            // get all the vertices
            Vector3 mainPos = vertexPath.GetMainPos(vertexIndex);
            Vector3 p1 = vertexPath.GetControlPos(vertexIndex, 1);
            Vector3 p2 = vertexPath.GetControlPos(vertexIndex, 2);

            void DrawControlPoint(int c)
            {
                Handles.DrawLine(c == 1 ? p1 : p2, mainPos);

                Color color = c == 1 ? Color.gray : new Color(0.35f, 0.35f, 0.35f);
                Handles.DrawLine(p2, mainPos);

                Vector3 location = DefineMovementGizmos(vertexPath.GetControlPos(vertexIndex, c), 0.2f, color, vertexIndex);
                Vector3 targetLocation = vertexPath.GetControlPos(vertexIndex, c);

                if (location != targetLocation)
                {
                    vertexPath.SetControlPos(vertexIndex, c, location, ControlHandleInt);
                }
            }

            if (nextIndex == -1)
            {
                DrawControlPoint(2);
                return;
            }

            Vector3 nextMainPos = vertexPath.GetMainPos(nextIndex);
            Vector3 nextp2 = vertexPath.GetControlPos(nextIndex, 2);

            // draw bezier curve!
            if (!pathCreator.isLinear)
            {
                DrawControlPoint(1);

                DrawControlPoint(2);

                Handles.zTest = CompareFunction.LessEqual;
                Handles.DrawBezier(mainPos, nextMainPos, p1, nextp2, Color.white, null, 10f);
            }
            else
            {
                Handles.zTest = CompareFunction.LessEqual;
                Handles.DrawLine(mainPos, nextMainPos, 4f);
            }
            Handles.zTest = CompareFunction.Always;
        }

        private Vector3 DefineMovementGizmos(Vector3 currVertexPos, float gizmoRadius, Color gizmoColor, int index)
        {
            Handles.color = gizmoColor;
            Vector3 location = currVertexPos;

            if (Event.current.capsLock)
            {
                location = Handles.PositionHandle(currVertexPos, Quaternion.identity);

                vertexPath.SetIsBeingMoved(index, location != currVertexPos);
            }
            else
            {
                location = Handles.FreeMoveHandle(currVertexPos, gizmoRadius, PathCreationSettings.defaultSnap, Handles.SphereHandleCap);
            }

            Handles.color = PathCreationSettings.defaultColor;

            // if location is the same (i.e. the handle is not selected) then continue to the next vertex
            if (currVertexPos == location)
            {
                return location;
            }

            if (GUI.changed)
            {
                return ApplySnapping(location, currVertexPos);
            }

            return location;
        }

        private Vector3 ApplySnapping(Vector3 location, Vector3 flatPlaneReferenceVector)
        {
            // snap to mesh
            if (ToolbarInt == 1)
            {
                return FindClosestPointOnMesh(location);
            }
            // snap flat (keep y the same)
            else if (ToolbarInt == 2)
            {
                Vector2 mousePos = GetMouseScreenPos();
                Ray ray = cam.ScreenPointToRay(mousePos);

                float desiredY = flatPlaneReferenceVector.y;

                if (Mathf.Abs(ray.direction.y) > 0.0001f) // Avoid dividing by zero or near-zero
                {
                    float t = (desiredY - ray.origin.y) / ray.direction.y; // Solve for t

                    if (t >= 0) // Only consider intersections in the ray's forward direction
                    {
                        Vector3 intersectionPoint = ray.origin + t * ray.direction;
                        return intersectionPoint;
                    }
                    else
                    {
                        Debug.Log("The intersection is behind the ray's origin");
                        return new Vector3(location.x, flatPlaneReferenceVector.y, location.z); ;
                    }
                }
                else
                {
                    Debug.Log("Ray is parallel to the plane at y = " + desiredY);
                    return new Vector3(location.x, flatPlaneReferenceVector.y, location.z); ;
                }
            }
            else
            {
                return location;
            }
        }

        private Vector3 FindClosestPointOnMesh(Vector3 p)
        {
            if (pathCreator.meshFilter == null)
            {
                Debug.LogWarning("You need to assign a mesh filter to enable mesh snapping");
                return p;
            }

            Vector2 mousePos = GetMouseScreenPos();

            RaycastHit[] hits = Physics.RaycastAll(cam.ScreenPointToRay(mousePos));

            RaycastHit actualHit = new RaycastHit();
            bool foundHit = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform == pathCreator.meshFilter.transform)
                {
                    actualHit = hit;
                    foundHit = true;
                }
            }

            if (foundHit)
            {
                 return actualHit.point;
            }
            else
            {
                p -= pathCreator.meshFilter.transform.position;

                Mesh mesh = pathCreator.meshFilter.sharedMesh;

                Vector3 closestVertex = new Vector3();
                float minDist = float.PositiveInfinity;

                foreach (Vector3 vertex in mesh.vertices)
                {
                    float d = Vector3.Distance(p, vertex);
                    if (d < minDist)
                    {
                        minDist = d;
                        closestVertex = vertex;
                    }
                }

                return closestVertex + pathCreator.meshFilter.transform.position;
            }
        }
        
        private Vector2 GetMouseScreenPos()
        {
            return HandleUtility.GUIPointToScreenPixelCoordinate(Event.current.mousePosition);
        }

        private Vector3 GetLastVector()
        {
            return vertexPath.GetMainPos(vertexPath.VertexCount() - 1);
        }

        int ToolbarInt
        {
            get { return toolbarInt; }
            set
            {
                toolbarInt = value;
                switch (value)
                {
                    case 0:
                        PathCreationSettings.sphereColor = Color.red;
                        break;

                    case 1:
                        PathCreationSettings.sphereColor = Color.blue;
                        break;

                    case 2:
                        PathCreationSettings.sphereColor = Color.green;
                        break;

                    default:
                        break;
                }

                SceneView.RepaintAll();
                Repaint();
            }
        }
        private int toolbarInt;

        int ControlHandleInt
        {
            get { return handleInt; }
            set
            {
                handleInt = value;

                SceneView.RepaintAll();
                Repaint();
            }
        }
        private int handleInt;
    }
}


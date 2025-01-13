using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[Serializable]
public class Range2d
{
    [Range(0f, 1f)] public float left;
    [Range(0f, 1f)] public float right;
    [Range(0f, 1f)] public float bottom;
    [Range(0f, 1f)] public float top;

    public Range2d(float left, float right, float bottom, float top) {
        this.left = left;
        this.right = right;
        this.bottom = bottom;
        this.top = top;
    }

    public bool IsValid() {
        return left < right && bottom < top && left + (1f - right) <= 1f && bottom + (1f - top) <= 1f;
    }

    public bool IsWithinBounds(float a, float b) {
        return left <= a && a <= right && bottom <= b && b <= top;
    }

    public override string ToString() {
        return "(left: " + left + ", right: " + right + ", bottom: " + bottom + ", top: " + top + ")";
    }
}

[Serializable]
public struct CinemaSettings
{
    public bool mode;
    public float speed;
    public Vector3 direction;
}

[Serializable]
public struct ZoomSettings
{
    public bool enable;
    [Range(0f, 1f)] public float zoom;
    public Vector2 fieldOfViewBounds;
}

/// <summary>
/// The initial Transform values set in the Inspector for the Camera are used to follow the player.
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    private GameObject player;
    private Camera mainCamera;

    private Vector3 cameraVelocity;
    private Vector3 cameraDirection;
    private Vector3 ogCamPos;
    private float distanceFromPlayer;

    [SerializeField] private bool debugLines;
    [SerializeField] private bool isDead;
    [SerializeField] private Range2d range;
    [SerializeField] private CinemaSettings cinema;
    [SerializeField] private ZoomSettings zm;

    private void Awake() {
        Debug.Assert(range.IsValid(), "Camera bounds are invalid!");

        player = GameObject.FindWithTag("Player");
        mainCamera = Camera.main;

        var playerPos = player.transform.Find("Body").transform.position;
        var cameraPos = mainCamera!.transform.position;

        distanceFromPlayer = Vector3.Distance(playerPos, cameraPos);
        cameraDirection = Vector3.Normalize(cameraPos - playerPos);
        ogCamPos = cameraPos;

        //SetZoom();
    }

    public void Kill(float duration) {
        Debug.Log("Player died, initiate camera death sequence");
        StartCoroutine(SmoothMoveCamera(ogCamPos, duration));
        isDead = true;
    }

    private IEnumerator SmoothMoveCamera(Vector3 endPos, float duration) {
        var elapsedTime = 0f;
        var initialCamPos = mainCamera.transform.position;

        // Animation duration should be a little faster
        var animDuration = duration - 0.1f;

        while (elapsedTime < animDuration) {
            var t = Easing.InOutPower(elapsedTime / animDuration, 5);
            mainCamera.transform.position = Vector3.Slerp(initialCamPos, ogCamPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = endPos;
    }

    private void SetZoom() {
        mainCamera.fieldOfView = Mathf.Lerp(zm.fieldOfViewBounds.x, zm.fieldOfViewBounds.y, 1f - zm.zoom);
    }
    
    private void Update() {
        if (cinema.mode)
        {
            mainCamera.transform.position += Vector3.Normalize(cinema.direction) * (Time.deltaTime * cinema.speed);
            return;
        }

        if (isDead) return;

        Debug.Assert(range.IsValid(), "Camera bounds are invalid!");

        // Zoom in and out

        if (zm.enable)
        {
            var scrollInput = Input.GetAxis("Mouse ScrollWheel");
            zm.zoom = Mathf.Clamp(zm.zoom + scrollInput, 0f, 1f);
            SetZoom();
        }

        if (player == null) return;

        var playerPos = player.transform.Find("Body").transform.position;
        var cameraPos = mainCamera.transform.position;

        var currDistanceFromPlayer = Vector3.Distance(playerPos, cameraPos);
        var distanceChanged = Mathf.Approximately(currDistanceFromPlayer, distanceFromPlayer);
        
        if (debugLines) {
            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(range.left, 1f, 1f)),
                           mainCamera.ViewportToWorldPoint(new Vector3(range.left, 0f, 1f)),
                           Color.white);

            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(range.right, 1f, 1f)),
                           mainCamera.ViewportToWorldPoint(new Vector3(range.right, 0f, 1f)),
                           Color.white);

            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(0, range.bottom, 1f)),
                           mainCamera.ViewportToWorldPoint(new Vector3(1f, range.bottom, 1f)),
                           Color.white);

            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(0, range.top, 1f)),
                           mainCamera.ViewportToWorldPoint(new Vector3(1f, range.top, 1f)),
                           Color.white);
        }

        var viewPos = mainCamera.WorldToViewportPoint(playerPos);

        // Only update camera position if the distance to the camera has changed, or the tank has gone
        // out of bounds. Otherwise, we return early
        if (range.IsWithinBounds(viewPos.x, viewPos.y) && !distanceChanged) return;

        var newPosition = playerPos + Vector3.Normalize(cameraDirection) * distanceFromPlayer;
        mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, newPosition, Time.deltaTime);
    }
}
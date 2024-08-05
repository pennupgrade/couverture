using System;
using UnityEngine;

[Serializable] public class Range2d
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
        return (left <= a && a <= right) && (bottom <= b && b <= top);
    }

    public override string ToString() {
        return "(left: " + left + ", right: " + right + ", bottom: " + bottom + ", top: " + top + ")";
    }
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
    private float distanceFromPlayer;

    [SerializeField] private bool debugLines;
    [SerializeField] private Range2d range;

    // For some reason the vertical debug lines aren't exact, so we offset them a little
    private const float VERTICAL_OFFSET = 0.025f;
    
    private void Awake() {
        Debug.Assert(range.IsValid(), "Camera bounds are invalid!");
        
        player = GameObject.FindWithTag("Player");
        mainCamera = Camera.main;

        var playerPos = player.transform.position;
        var cameraPos = mainCamera!.transform.position;

        distanceFromPlayer = Vector3.Distance(playerPos, cameraPos);
        cameraDirection = Vector3.Normalize(cameraPos - playerPos);
    }

    private void Update() {
        Debug.Assert(range.IsValid(), "Camera bounds are invalid!");
        
        var playerPosition = player.transform.position;
        
        if (debugLines) {
            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(range.left, 1f, 1f)), 
                           mainCamera.ViewportToWorldPoint(new Vector3(range.left, 0f, 1f)), 
                           Color.white);
            
            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(range.right, 1f, 1f)), 
                           mainCamera.ViewportToWorldPoint(new Vector3(range.right, 0f, 1f)), 
                           Color.white);
            
            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(0, range.bottom + VERTICAL_OFFSET, 1f)), 
                           mainCamera.ViewportToWorldPoint(new Vector3(1f, range.bottom + VERTICAL_OFFSET, 1f)), 
                           Color.white);
            
            Debug.DrawLine(mainCamera.ViewportToWorldPoint(new Vector3(0, range.top + VERTICAL_OFFSET, 1f)), 
                           mainCamera.ViewportToWorldPoint(new Vector3(1f, range.top + VERTICAL_OFFSET, 1f)), 
                           Color.white);
        }
        
        var viewPos = mainCamera.WorldToViewportPoint(playerPosition);
        if (range.IsWithinBounds(viewPos.x, viewPos.y)) return;
        
        var newPosition = playerPosition + (Vector3.Normalize(cameraDirection) * distanceFromPlayer);
        mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, newPosition, Time.deltaTime);
    }
}